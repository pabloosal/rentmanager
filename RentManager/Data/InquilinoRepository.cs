using Microsoft.Data.Sqlite;
using RentManager.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace RentManager.Data
{
    // Clase encargada de gestionar el acceso a datos de Inquilino
    public class InquilinoRepository
    {
        // Método que devuelve el listado completo de inquilinos
        public List<Inquilino> ObtenerTodos()
        {
            var lista = new List<Inquilino>();

            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            var query = @"
                SELECT id_inquilino, nombre, apellidos, dni, telefono, email, observaciones, fecha_alta
                FROM Inquilino
                ORDER BY id_inquilino DESC;
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var inquilino = new Inquilino
                {
                    IdInquilino = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Apellidos = reader.GetString(2),
                    Dni = reader.GetString(3),
                    Telefono = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    Email = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    Observaciones = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    FechaAlta = DateTime.Parse(reader.GetString(7), CultureInfo.InvariantCulture)
                };

                lista.Add(inquilino);
            }

            return lista;
        }

        // Método que inserta un inquilino nuevo en la base de datos
        public void Insertar(Inquilino inquilino)
        {
            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            var query = @"
                INSERT INTO Inquilino (nombre, apellidos, dni, telefono, email, observaciones, fecha_alta)
                VALUES (@nombre, @apellidos, @dni, @telefono, @email, @obs, @fecha);
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;

            cmd.Parameters.AddWithValue("@nombre", inquilino.Nombre);
            cmd.Parameters.AddWithValue("@apellidos", inquilino.Apellidos);
            cmd.Parameters.AddWithValue("@dni", inquilino.Dni);

            cmd.Parameters.AddWithValue("@telefono",
                string.IsNullOrWhiteSpace(inquilino.Telefono) ? (object)DBNull.Value : inquilino.Telefono);

            cmd.Parameters.AddWithValue("@email",
                string.IsNullOrWhiteSpace(inquilino.Email) ? (object)DBNull.Value : inquilino.Email);

            cmd.Parameters.AddWithValue("@obs",
                string.IsNullOrWhiteSpace(inquilino.Observaciones) ? (object)DBNull.Value : inquilino.Observaciones);

            cmd.Parameters.AddWithValue("@fecha", inquilino.FechaAlta.ToString("s"));

            cmd.ExecuteNonQuery();
        }

        // Método para actualizar un inquilino existente
        public void Actualizar(Inquilino inquilino)
        {
            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            var query = @"
                UPDATE Inquilino
                SET nombre = @nombre,
                    apellidos = @apellidos,
                    dni = @dni,
                    telefono = @telefono,
                    email = @email,
                    observaciones = @obs
                WHERE id_inquilino = @id;
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;

            cmd.Parameters.AddWithValue("@nombre", inquilino.Nombre);
            cmd.Parameters.AddWithValue("@apellidos", inquilino.Apellidos);
            cmd.Parameters.AddWithValue("@dni", inquilino.Dni);

            cmd.Parameters.AddWithValue("@telefono",
                string.IsNullOrWhiteSpace(inquilino.Telefono) ? (object)DBNull.Value : inquilino.Telefono);

            cmd.Parameters.AddWithValue("@email",
                string.IsNullOrWhiteSpace(inquilino.Email) ? (object)DBNull.Value : inquilino.Email);

            cmd.Parameters.AddWithValue("@obs",
                string.IsNullOrWhiteSpace(inquilino.Observaciones) ? (object)DBNull.Value : inquilino.Observaciones);

            cmd.Parameters.AddWithValue("@id", inquilino.IdInquilino);

            cmd.ExecuteNonQuery();
        }

        // Método para eliminar un inquilino por su ID
        public void Eliminar(int idInquilino)
        {
            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            var query = @"DELETE FROM Inquilino WHERE id_inquilino = @id;";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@id", idInquilino);

            cmd.ExecuteNonQuery();
        }
    }
}
