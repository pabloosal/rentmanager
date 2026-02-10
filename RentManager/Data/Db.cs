using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace RentManager.Data
{
    /// Clase encargada de gestionar la base de datos SQLite.
    /// Se ocupa de crear la base de datos, las tablas necesarias y de inicializar los datos básicos del sistema.
    public static class Db
    {
        // Carpeta donde se almacenará la base de datos dentro del directorio del usuario
        private static readonly string DbFolder =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RentManager"
            );

        // Ruta completa del archivo de base de datos SQLite
        private static readonly string DbPath =
            Path.Combine(DbFolder, "rentmanager.db");

        // Cadena de conexión a la base de datos
        public static string ConnectionString => $"Data Source={DbPath}";

        /// Inicializa la base de datos del sistema.
        public static void Initialize()
        {
            // Crear la carpeta de la base de datos si no existe
            if (!Directory.Exists(DbFolder))
            {
                Directory.CreateDirectory(DbFolder);
            }

            // Abrir conexión con la base de datos SQLite
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            // Crear la tabla Usuario si no existe
            // Creación de tablas y datos iniciales
            CrearTablaUsuario(connection);
            InsertarUsuarioAdministrador(connection);

            // Insertar un usuario administrador por defecto si no existe
            CrearTablaVivienda(connection);
            InsertarViviendaEjemplo(connection);
        }

        // Crea la tabla Usuario si no existe
        private static void CrearTablaUsuario(SqliteConnection connection)
        {
            var sql = @"
                CREATE TABLE IF NOT EXISTS Usuario (
                    id_usuario    INTEGER PRIMARY KEY AUTOINCREMENT,
                    nombre        TEXT NOT NULL,
                    email         TEXT NOT NULL UNIQUE,
                    password_hash TEXT NOT NULL
                );
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        // Inserta un usuario administrador por defecto si no existe
        private static void InsertarUsuarioAdministrador(SqliteConnection connection)
        {
            var sql = @"
                INSERT INTO Usuario (nombre, email, password_hash)
                SELECT 'Administrador', 'admin@rentmanager.com', 'admin123'
                WHERE NOT EXISTS (
                    SELECT 1 FROM Usuario WHERE email = 'admin@rentmanager.com'
                );
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        // Crea la tabla Vivienda si no existe
        private static void CrearTablaVivienda(SqliteConnection connection)
        {
            var sql = @"
                CREATE TABLE IF NOT EXISTS Vivienda (
                    id_vivienda     INTEGER PRIMARY KEY AUTOINCREMENT,
                    direccion       TEXT NOT NULL,
                    ciudad          TEXT NOT NULL,
                    codigo_postal   TEXT NOT NULL,
                    precio_mensual  REAL NOT NULL,
                    estado          TEXT NOT NULL,
                    observaciones   TEXT,
                    fecha_alta      TEXT NOT NULL
                );
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        // Inserta una vivienda de ejemplo si no existe ninguna (solo para pruebas)
        private static void InsertarViviendaEjemplo(SqliteConnection connection)
        {
            var sql = @"
                INSERT INTO Vivienda (direccion, ciudad, codigo_postal, precio_mensual, estado, observaciones, fecha_alta)
                SELECT 'Calle Ejemplo 123', 'A Coruña', '15001', 750, 'Libre', 'Vivienda de prueba', datetime('now')
                WHERE NOT EXISTS (SELECT 1 FROM Vivienda);
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}
