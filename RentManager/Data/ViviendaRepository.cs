using Microsoft.Data.Sqlite;
using RentManager.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace RentManager.Data
{
    // Clase encargada de gestionar el acceso a datos de Vivienda
    public class ViviendaRepository
    {
        // Método que devuelve el listado completo de viviendas
        public List<Vivienda> ObtenerTodas()
        {
            var lista = new List<Vivienda>();

            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            var query = @"
                SELECT id_vivienda, direccion, ciudad, codigo_postal, precio_mensual, estado, observaciones, fecha_alta
                FROM Vivienda
                ORDER BY id_vivienda DESC;
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var vivienda = new Vivienda
                {
                    IdVivienda = reader.GetInt32(0),
                    Direccion = reader.GetString(1),
                    Ciudad = reader.GetString(2),
                    CodigoPostal = reader.GetString(3),
                    PrecioMensual = Convert.ToDecimal(reader.GetDouble(4)),
                    Estado = reader.GetString(5),
                    Observaciones = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    FechaAlta = DateTime.Parse(reader.GetString(7), CultureInfo.InvariantCulture)
                };

                lista.Add(vivienda);
            }

            return lista;
        }

        // Método que inserta una vivienda nueva en la base de datos
        public void Insertar(Vivienda vivienda)
        {
            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            var query = @"
                INSERT INTO Vivienda (direccion, ciudad, codigo_postal, precio_mensual, estado, observaciones, fecha_alta)
                VALUES (@direccion, @ciudad, @cp, @precio, @estado, @obs, @fecha);
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;

            cmd.Parameters.AddWithValue("@direccion", vivienda.Direccion);
            cmd.Parameters.AddWithValue("@ciudad", vivienda.Ciudad);
            cmd.Parameters.AddWithValue("@cp", vivienda.CodigoPostal);
            cmd.Parameters.AddWithValue("@precio", vivienda.PrecioMensual);
            cmd.Parameters.AddWithValue("@estado", vivienda.Estado);
            cmd.Parameters.AddWithValue("@obs", string.IsNullOrWhiteSpace(vivienda.Observaciones) ? (object)DBNull.Value : vivienda.Observaciones);
            cmd.Parameters.AddWithValue("@fecha", vivienda.FechaAlta.ToString("s"));

            cmd.ExecuteNonQuery();
        }

        // Método para actualizar una vivienda existente
        public void Actualizar(Vivienda vivienda)
        {
            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            var query = @"
                UPDATE Vivienda
                SET direccion = @direccion,
                    ciudad = @ciudad,
                    codigo_postal = @cp,
                    precio_mensual = @precio,
                    estado = @estado,
                    observaciones = @obs
                WHERE id_vivienda = @id;
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;

            cmd.Parameters.AddWithValue("@direccion", vivienda.Direccion);
            cmd.Parameters.AddWithValue("@ciudad", vivienda.Ciudad);
            cmd.Parameters.AddWithValue("@cp", vivienda.CodigoPostal);
            cmd.Parameters.AddWithValue("@precio", vivienda.PrecioMensual);
            cmd.Parameters.AddWithValue("@estado", vivienda.Estado);
            cmd.Parameters.AddWithValue("@obs", string.IsNullOrWhiteSpace(vivienda.Observaciones) ? (object)DBNull.Value : vivienda.Observaciones);
            cmd.Parameters.AddWithValue("@id", vivienda.IdVivienda);

            cmd.ExecuteNonQuery();
        }

        // Método para eliminar una vivienda por su ID
        public void Eliminar(int idVivienda)
        {
            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            var query = @"DELETE FROM Vivienda WHERE id_vivienda = @id;";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", idVivienda);

            cmd.ExecuteNonQuery();
        }
    }
}
