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

            // Activa el uso de claves foráneas en SQLite
            using (var pragma = connection.CreateCommand())
            {
                pragma.CommandText = "PRAGMA foreign_keys = ON;";
                pragma.ExecuteNonQuery();
            }

            // Crear la tabla Usuario si no existe
            // Creación de tablas y datos iniciales
            CrearTablaUsuario(connection);
            InsertarUsuarioAdministrador(connection);

            // Insertar un usuario administrador por defecto si no existe
            CrearTablaVivienda(connection);
            InsertarViviendaEjemplo(connection);

            // Crear la tabla Inquilino si no existe
            CrearTablaInquilino(connection);
            InsertarInquilinoEjemplo(connection);

            //Crear tabla contrato si no existe
            CrearTablaContrato(connection);
            InsertarContratoEjemplo(connection);

            // Crear tabla Pago si no existe
            CrearTablaPago(connection);
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

        // Crea la tabla Inquilino si no existe
        private static void CrearTablaInquilino(SqliteConnection connection)
        {
            var sql = @"
                CREATE TABLE IF NOT EXISTS Inquilino (
                    id_inquilino   INTEGER PRIMARY KEY AUTOINCREMENT,
                    nombre         TEXT NOT NULL,
                    apellidos      TEXT NOT NULL,
                    dni            TEXT NOT NULL UNIQUE,
                    telefono       TEXT,
                    email          TEXT,
                    observaciones  TEXT,
                    fecha_alta     TEXT NOT NULL
                );
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        // Inserta un inquilino de ejemplo si no existe ninguno (solo para pruebas)
        private static void InsertarInquilinoEjemplo(SqliteConnection connection)
        {
            var sql = @"
                INSERT INTO Inquilino (nombre, apellidos, dni, telefono, email, observaciones, fecha_alta)
                SELECT 'Juan', 'Pérez', '12345678A', '600111222', 'juan.perez@email.com', 'Inquilino de prueba', datetime('now')
                WHERE NOT EXISTS (SELECT 1 FROM Inquilino);
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        // Crea la tabla Contrato si no existe
        private static void CrearTablaContrato(SqliteConnection connection)
        {
            var sql = @"
                CREATE TABLE IF NOT EXISTS Contrato (
                    id_contrato    INTEGER PRIMARY KEY AUTOINCREMENT,
                    id_vivienda    INTEGER NOT NULL,
                    id_inquilino   INTEGER NOT NULL,
                    fecha_inicio   TEXT NOT NULL,
                    fecha_fin      TEXT,
                    renta_mensual  REAL NOT NULL,
                    fianza         REAL,
                    estado         TEXT NOT NULL,
                    observaciones  TEXT,
                    FOREIGN KEY (id_vivienda) REFERENCES Vivienda(id_vivienda),
                    FOREIGN KEY (id_inquilino) REFERENCES Inquilino(id_inquilino)
                );
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        // Inserta un contrato de ejemplo si hay datos y no existe ningún contrato
        private static void InsertarContratoEjemplo(SqliteConnection connection)
        {
            var sql = @"
        INSERT INTO Contrato (id_vivienda, id_inquilino, fecha_inicio, fecha_fin, renta_mensual, fianza, estado, observaciones)
        SELECT 
            (SELECT id_vivienda FROM Vivienda ORDER BY id_vivienda ASC LIMIT 1),
            (SELECT id_inquilino FROM Inquilino ORDER BY id_inquilino ASC LIMIT 1),
            datetime('now'),
            NULL,
            750,
            750,
            'Activo',
            'Contrato de ejemplo'
                WHERE 
                    (SELECT COUNT(*) FROM Contrato) = 0
                    AND (SELECT COUNT(*) FROM Vivienda) > 0
                    AND (SELECT COUNT(*) FROM Inquilino) > 0;
            ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        // Crea la tabla Pago si no existe
        private static void CrearTablaPago(SqliteConnection connection)
        {
            var sql = @"
        CREATE TABLE IF NOT EXISTS Pago (
            id_pago     INTEGER PRIMARY KEY AUTOINCREMENT,
            id_contrato INTEGER NOT NULL,
            mes         INTEGER NOT NULL,
            anyo        INTEGER NOT NULL,
            importe     REAL NOT NULL,
            estado      TEXT NOT NULL,
            fecha_pago  TEXT,
            observaciones TEXT,
            FOREIGN KEY (id_contrato) REFERENCES Contrato(id_contrato),
            UNIQUE (id_contrato, mes, anyo)
            );
        ";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }


    }
}
