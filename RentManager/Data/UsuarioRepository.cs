using Microsoft.Data.Sqlite;

namespace RentManager.Data
{
    // Clase encargada de gestionar las operaciones relacionadas con los usuarios
    public class UsuarioRepository
    {
        // Comprueba si las credenciales introducidas existen en la base de datos
        public bool ValidarLogin(string email, string password)
        {
            // Se abre la conexión con la base de datos SQLite
            using var connection = new SqliteConnection(Db.ConnectionString);
            connection.Open();

            // Consulta para comprobar si existe un usuario con ese email y contraseña
            var query = @"
                SELECT COUNT(*)
                FROM Usuario
                WHERE email = @email
                  AND password_hash = @password
            ";

            // Se prepara el comando con parámetros para evitar inyección SQL
            using var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password", password);

            // Si el resultado es mayor que 0, las credenciales son correctas
            var result = (long)cmd.ExecuteScalar();
            return result > 0;
        }
    }
}
