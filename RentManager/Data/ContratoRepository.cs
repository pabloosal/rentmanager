using Microsoft.Data.Sqlite;
using RentManager.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace RentManager.Data
{
    // Clase para mostrar contratos en listados (incluye datos legibles de vivienda e inquilino)
    public class ContratoListado
    {
        public int IdContrato { get; set; }
        public int IdVivienda { get; set; }
        public int IdInquilino { get; set; }

        public string ViviendaDireccion { get; set; } = string.Empty;
        public string InquilinoNombreCompleto { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public decimal RentaMensual { get; set; }
        public EstadoContrato Estado { get; set; }
    }

    // Clase encargada de gestionar el acceso a datos de Contrato
    public class ContratoRepository
    {
        // Devuelve el listado de contratos mostrando dirección e inquilino (JOIN)
        public List<ContratoListado> ObtenerTodos()
        {
            var lista = new List<ContratoListado>();

            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            // (Opcional recomendado) activa claves foráneas en SQLite
            using (var pragma = connection.CreateCommand())
            {
                pragma.CommandText = "PRAGMA foreign_keys = ON;";
                pragma.ExecuteNonQuery();
            }

            var query = @"
                SELECT
                    c.id_contrato,
                    c.id_vivienda,
                    c.id_inquilino,
                    v.direccion,
                    (i.nombre || ' ' || i.apellidos) AS inquilino,
                    c.fecha_inicio,
                    c.fecha_fin,
                    c.renta_mensual,
                    c.estado
                FROM Contrato c
                JOIN Vivienda v ON c.id_vivienda = v.id_vivienda
                JOIN Inquilino i ON c.id_inquilino = i.id_inquilino
                ORDER BY c.id_contrato DESC;
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var estadoTexto = reader.GetString(8);
                Enum.TryParse(estadoTexto, out EstadoContrato estado);

                lista.Add(new ContratoListado
                {
                    IdContrato = reader.GetInt32(0),
                    IdVivienda = reader.GetInt32(1),
                    IdInquilino = reader.GetInt32(2),
                    ViviendaDireccion = reader.GetString(3),
                    InquilinoNombreCompleto = reader.GetString(4),
                    FechaInicio = DateTime.Parse(reader.GetString(5), CultureInfo.InvariantCulture),
                    FechaFin = reader.IsDBNull(6) ? (DateTime?)null : DateTime.Parse(reader.GetString(6), CultureInfo.InvariantCulture),
                    RentaMensual = Convert.ToDecimal(reader.GetDouble(7)),
                    Estado = estado
                });
            }

            return lista;
        }

        // Inserta un contrato nuevo
        public void Insertar(Contrato contrato)
        {
            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            using (var pragma = connection.CreateCommand())
            {
                pragma.CommandText = "PRAGMA foreign_keys = ON;";
                pragma.ExecuteNonQuery();
            }

            var query = @"
                INSERT INTO Contrato
                    (id_vivienda, id_inquilino, fecha_inicio, fecha_fin, renta_mensual, fianza, estado, observaciones)
                VALUES
                    (@idVivienda, @idInquilino, @inicio, @fin, @renta, @fianza, @estado, @obs);
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;

            cmd.Parameters.AddWithValue("@idVivienda", contrato.IdVivienda);
            cmd.Parameters.AddWithValue("@idInquilino", contrato.IdInquilino);

            cmd.Parameters.AddWithValue("@inicio", contrato.FechaInicio.ToString("s"));

            cmd.Parameters.AddWithValue("@fin",
                contrato.FechaFin.HasValue ? contrato.FechaFin.Value.ToString("s") : (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@renta", contrato.RentaMensual);

            cmd.Parameters.AddWithValue("@fianza",
                contrato.Fianza.HasValue ? contrato.Fianza.Value : (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@estado", contrato.Estado.ToString());

            cmd.Parameters.AddWithValue("@obs",
                string.IsNullOrWhiteSpace(contrato.Observaciones) ? (object)DBNull.Value : contrato.Observaciones);

            cmd.ExecuteNonQuery();
        }

        // Actualiza un contrato existente
        public void Actualizar(Contrato contrato)
        {
            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            using (var pragma = connection.CreateCommand())
            {
                pragma.CommandText = "PRAGMA foreign_keys = ON;";
                pragma.ExecuteNonQuery();
            }

            var query = @"
                UPDATE Contrato
                SET id_vivienda = @idVivienda,
                    id_inquilino = @idInquilino,
                    fecha_inicio = @inicio,
                    fecha_fin = @fin,
                    renta_mensual = @renta,
                    fianza = @fianza,
                    estado = @estado,
                    observaciones = @obs
                WHERE id_contrato = @idContrato;
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;

            cmd.Parameters.AddWithValue("@idVivienda", contrato.IdVivienda);
            cmd.Parameters.AddWithValue("@idInquilino", contrato.IdInquilino);

            cmd.Parameters.AddWithValue("@inicio", contrato.FechaInicio.ToString("s"));
            cmd.Parameters.AddWithValue("@fin",
                contrato.FechaFin.HasValue ? contrato.FechaFin.Value.ToString("s") : (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@renta", contrato.RentaMensual);
            cmd.Parameters.AddWithValue("@fianza",
                contrato.Fianza.HasValue ? contrato.Fianza.Value : (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@estado", contrato.Estado.ToString());
            cmd.Parameters.AddWithValue("@obs",
                string.IsNullOrWhiteSpace(contrato.Observaciones) ? (object)DBNull.Value : contrato.Observaciones);

            cmd.Parameters.AddWithValue("@idContrato", contrato.IdContrato);

            cmd.ExecuteNonQuery();
        }

        // Elimina un contrato por su ID
        public void Eliminar(int idContrato)
        {
            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            using (var pragma = connection.CreateCommand())
            {
                pragma.CommandText = "PRAGMA foreign_keys = ON;";
                pragma.ExecuteNonQuery();
            }

            var query = @"DELETE FROM Contrato WHERE id_contrato = @id;";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", idContrato);

            cmd.ExecuteNonQuery();
        }
    }
}
