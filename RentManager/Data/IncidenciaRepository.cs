using Microsoft.Data.Sqlite;
using RentManager.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace RentManager.Data
{
    // Clase utilizada para mostrar incidencias en el listado
    public class IncidenciaListado
    {
        public int IdIncidencia { get; set; }
        public int IdVivienda { get; set; }

        public string ViviendaDireccion { get; set; } = string.Empty;

        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }

        public EstadoIncidencia Estado { get; set; }

        public decimal CosteEstimado { get; set; }

        public string Observaciones { get; set; } = string.Empty;
    }

    // Gestiona el acceso a datos de incidencias
    public class IncidenciaRepository
    {
        // Obtiene todas las incidencias
        public List<IncidenciaListado> ObtenerTodas()
        {
            var lista = new List<IncidenciaListado>();

            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            var query = @"
                SELECT
                    i.id_incidencia,
                    i.id_vivienda,
                    v.direccion,
                    i.titulo,
                    i.descripcion,
                    i.fecha,
                    i.estado,
                    i.coste_estimado,
                    i.observaciones
                FROM Incidencia i
                JOIN Vivienda v ON i.id_vivienda = v.id_vivienda
                ORDER BY i.fecha DESC;
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Enum.TryParse(reader.GetString(6), out EstadoIncidencia estado);

                lista.Add(new IncidenciaListado
                {
                    IdIncidencia = reader.GetInt32(0),
                    IdVivienda = reader.GetInt32(1),
                    ViviendaDireccion = reader.GetString(2),
                    Titulo = reader.GetString(3),
                    Descripcion = reader.GetString(4),
                    Fecha = DateTime.Parse(reader.GetString(5), CultureInfo.InvariantCulture),
                    Estado = estado,
                    CosteEstimado = Convert.ToDecimal(reader.GetDouble(7)),
                    Observaciones = reader.IsDBNull(8) ? "" : reader.GetString(8)
                });
            }

            return lista;
        }

        // Inserta una incidencia
        public void Insertar(Incidencia incidencia)
        {
            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            var query = @"
                INSERT INTO Incidencia
                (
                    id_vivienda,
                    titulo,
                    descripcion,
                    fecha,
                    estado,
                    coste_estimado,
                    observaciones
                )
                VALUES
                (
                    @idVivienda,
                    @titulo,
                    @descripcion,
                    @fecha,
                    @estado,
                    @coste,
                    @observaciones
                );
            ";

            using var cmd = connection.CreateCommand();

            cmd.CommandText = query;

            cmd.Parameters.AddWithValue("@idVivienda", incidencia.IdVivienda);
            cmd.Parameters.AddWithValue("@titulo", incidencia.Titulo);
            cmd.Parameters.AddWithValue("@descripcion", incidencia.Descripcion);
            cmd.Parameters.AddWithValue("@fecha", incidencia.Fecha.ToString("s"));
            cmd.Parameters.AddWithValue("@estado", incidencia.Estado.ToString());
            cmd.Parameters.AddWithValue("@coste", incidencia.CosteEstimado);
            cmd.Parameters.AddWithValue("@observaciones", incidencia.Observaciones);

            cmd.ExecuteNonQuery();
        }

        // Elimina una incidencia
        public void Eliminar(int idIncidencia)
        {
            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            var query = "DELETE FROM Incidencia WHERE id_incidencia = @id";

            using var cmd = connection.CreateCommand();

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", idIncidencia);

            cmd.ExecuteNonQuery();
        }

        // Actualiza una incidencia existente
        public void Actualizar(Incidencia incidencia)
        {
            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            var query = @"
        UPDATE Incidencia
        SET id_vivienda = @idVivienda,
            titulo = @titulo,
            descripcion = @descripcion,
            fecha = @fecha,
            estado = @estado,
            coste_estimado = @coste,
            observaciones = @observaciones
        WHERE id_incidencia = @id;
    ";

            using var cmd = connection.CreateCommand();

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@idVivienda", incidencia.IdVivienda);
            cmd.Parameters.AddWithValue("@titulo", incidencia.Titulo);
            cmd.Parameters.AddWithValue("@descripcion", incidencia.Descripcion);
            cmd.Parameters.AddWithValue("@fecha", incidencia.Fecha.ToString("s"));
            cmd.Parameters.AddWithValue("@estado", incidencia.Estado.ToString());
            cmd.Parameters.AddWithValue("@coste", incidencia.CosteEstimado);
            cmd.Parameters.AddWithValue("@observaciones", incidencia.Observaciones);
            cmd.Parameters.AddWithValue("@id", incidencia.IdIncidencia);

            cmd.ExecuteNonQuery();
        }
    }
}