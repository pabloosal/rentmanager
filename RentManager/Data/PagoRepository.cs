using Microsoft.Data.Sqlite;
using RentManager.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace RentManager.Data
{
    // Clase para mostrar pagos en listados (incluye datos legibles del contrato)
    public class PagoListado
    {
        public int IdPago { get; set; }
        public int IdContrato { get; set; }

        public string ViviendaDireccion { get; set; } = string.Empty;
        public string InquilinoNombreCompleto { get; set; } = string.Empty;

        public int Mes { get; set; }
        public int Anyo { get; set; }

        public decimal Importe { get; set; }
        public EstadoPago Estado { get; set; }
        public DateTime? FechaPago { get; set; }
    }

    // Clase encargada de gestionar el acceso a datos de Pagos
    public class PagoRepository
    {
        // Devuelve pagos con JOIN para mostrar vivienda e inquilino
        public List<PagoListado> ObtenerTodos(int? idContrato = null, int? anyo = null, EstadoPago? estado = null)
        {
            var lista = new List<PagoListado>();

            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            using (var pragma = connection.CreateCommand())
            {
                pragma.CommandText = "PRAGMA foreign_keys = ON;";
                pragma.ExecuteNonQuery();
            }

            var query = @"
                SELECT
                    p.id_pago,
                    p.id_contrato,
                    v.direccion,
                    (i.nombre || ' ' || i.apellidos) AS inquilino,
                    p.mes,
                    p.anyo,
                    p.importe,
                    p.estado,
                    p.fecha_pago
                FROM Pago p
                JOIN Contrato c ON p.id_contrato = c.id_contrato
                JOIN Vivienda v ON c.id_vivienda = v.id_vivienda
                JOIN Inquilino i ON c.id_inquilino = i.id_inquilino
                WHERE 1=1
            ";

            if (idContrato.HasValue) query += " AND p.id_contrato = @idContrato";
            if (anyo.HasValue) query += " AND p.anyo = @anyo";
            if (estado.HasValue) query += " AND p.estado = @estado";

            query += " ORDER BY p.anyo DESC, p.mes DESC;";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;

            if (idContrato.HasValue) cmd.Parameters.AddWithValue("@idContrato", idContrato.Value);
            if (anyo.HasValue) cmd.Parameters.AddWithValue("@anyo", anyo.Value);
            if (estado.HasValue) cmd.Parameters.AddWithValue("@estado", estado.Value.ToString());

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var estadoTexto = reader.GetString(7);
                Enum.TryParse(estadoTexto, out EstadoPago est);

                lista.Add(new PagoListado
                {
                    IdPago = reader.GetInt32(0),
                    IdContrato = reader.GetInt32(1),
                    ViviendaDireccion = reader.GetString(2),
                    InquilinoNombreCompleto = reader.GetString(3),
                    Mes = reader.GetInt32(4),
                    Anyo = reader.GetInt32(5),
                    Importe = Convert.ToDecimal(reader.GetDouble(6)),
                    Estado = est,
                    FechaPago = reader.IsDBNull(8) ? (DateTime?)null : DateTime.Parse(reader.GetString(8), CultureInfo.InvariantCulture)
                });
            }

            return lista;
        }

        // Marca un pago como pagado y pone fecha de pago
        public void MarcarComoPagado(int idPago, DateTime fechaPago)
        {
            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            var query = @"
                UPDATE Pago
                SET estado = 'Pagado',
                    fecha_pago = @fecha
                WHERE id_pago = @id;
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@fecha", fechaPago.ToString("s"));
            cmd.Parameters.AddWithValue("@id", idPago);

            cmd.ExecuteNonQuery();
        }

        // Elimina un pago por su ID
        public void Eliminar(int idPago)
        {
            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            var query = "DELETE FROM Pago WHERE id_pago = @id;";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", idPago);

            cmd.ExecuteNonQuery();
        }

        // Genera pagos "Pendiente" para los meses del año seleccionado dentro del rango del contrato (sin duplicar)
        public void GenerarPagosMensuales(int idContrato, int anyo)
        {
            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            using (var pragma = connection.CreateCommand())
            {
                pragma.CommandText = "PRAGMA foreign_keys = ON;";
                pragma.ExecuteNonQuery();
            }

            // 1) Obtener datos del contrato
            var contrato = ObtenerDatosContrato(connection, idContrato);
            if (contrato == null)
                return;

            // 2) Calcular rango de meses dentro del año y dentro del contrato
            var inicioRango = new DateTime(anyo, 1, 1);
            var finRango = new DateTime(anyo, 12, 31);

            var inicio = contrato.Value.Inicio > inicioRango ? contrato.Value.Inicio : inicioRango;

            DateTime finContrato = contrato.Value.Fin ?? finRango;
            var fin = finContrato < finRango ? finContrato : finRango;

            if (fin < inicio)
                return; // ese contrato no afecta a ese año

            int mesInicio = inicio.Month;
            int mesFin = fin.Month;

            // 3) Insertar pagos pendientes por cada mes
            for (int mes = mesInicio; mes <= mesFin; mes++)
            {
                var insert = @"
                    INSERT OR IGNORE INTO Pago (id_contrato, mes, anyo, importe, estado, fecha_pago, observaciones)
                    VALUES (@idContrato, @mes, @anyo, @importe, 'Pendiente', NULL, NULL);
                ";

                using var cmd = connection.CreateCommand();
                cmd.CommandText = insert;
                cmd.Parameters.AddWithValue("@idContrato", idContrato);
                cmd.Parameters.AddWithValue("@mes", mes);
                cmd.Parameters.AddWithValue("@anyo", anyo);
                cmd.Parameters.AddWithValue("@importe", contrato.Value.Renta);

                cmd.ExecuteNonQuery();
            }
        }

        // Obtiene inicio, fin y renta del contrato
        private (DateTime Inicio, DateTime? Fin, decimal Renta)? ObtenerDatosContrato(SqliteConnection connection, int idContrato)
        {
            var query = @"
                SELECT fecha_inicio, fecha_fin, renta_mensual
                FROM Contrato
                WHERE id_contrato = @id;
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", idContrato);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            var inicio = DateTime.Parse(reader.GetString(0), CultureInfo.InvariantCulture);
            DateTime? fin = reader.IsDBNull(1) ? (DateTime?)null : DateTime.Parse(reader.GetString(1), CultureInfo.InvariantCulture);

            var renta = Convert.ToDecimal(reader.GetDouble(2));
            return (inicio, fin, renta);
        }
    }
}
