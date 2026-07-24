using BibliotecaApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BibliotecaApi.Data
{
    // Excepción para violaciones de reglas de negocio (el controlador la traduce a 400).
    public class ReglaNegocioException : Exception
    {
        public ReglaNegocioException(string mensaje) : base(mensaje) { }
    }

    public class PrestamoDAO
    {
        string connectionString =
            ConfigurationManager.ConnectionStrings["BibliotecaConnection"].ConnectionString;

        // Días de préstamo por defecto (configurable en Web.config).
        private static int DiasPrestamo
        {
            get
            {
                int d;
                return int.TryParse(ConfigurationManager.AppSettings["DiasPrestamo"], out d) ? d : 14;
            }
        }

        // GET ALL
        public List<Prestamo> GetAll()
        {
            List<Prestamo> lista = new List<Prestamo>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT id, libroId, usuarioId, fechaPrestamo, fechaLimite, fechaDevolucion, estado FROM Prestamo";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            lista.Add(Map(reader));
                    }
                }
            }
            return lista;
        }

        // GET BY ID
        public Prestamo GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT id, libroId, usuarioId, fechaPrestamo, fechaLimite, fechaDevolucion, estado FROM Prestamo WHERE id=@id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) return Map(reader);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Registra un préstamo: valida que el libro exista y esté disponible,
        /// calcula fechas y estado en el servidor y marca el libro como no disponible.
        /// Todo dentro de una transacción. Devuelve el id creado.
        /// </summary>
        public int Prestar(Prestamo p)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        if (!Existe(conn, tx, "Usuario", p.usuarioId))
                            throw new ReglaNegocioException("El usuario indicado no existe.");

                        bool? disponible = LibroDisponible(conn, tx, p.libroId);
                        if (disponible == null)
                            throw new ReglaNegocioException("El libro indicado no existe.");
                        if (disponible == false)
                            throw new ReglaNegocioException("El libro no está disponible para préstamo.");

                        int nuevoId;
                        string insert =
                            "INSERT INTO Prestamo(libroId, usuarioId, fechaPrestamo, fechaLimite, fechaDevolucion, estado) " +
                            "VALUES(@libroId, @usuarioId, GETDATE(), DATEADD(DAY, @dias, GETDATE()), NULL, 'Activo'); " +
                            "SELECT CAST(SCOPE_IDENTITY() AS int);";
                        using (SqlCommand cmd = new SqlCommand(insert, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@libroId", p.libroId);
                            cmd.Parameters.AddWithValue("@usuarioId", p.usuarioId);
                            cmd.Parameters.AddWithValue("@dias", DiasPrestamo);
                            nuevoId = (int)cmd.ExecuteScalar();
                        }

                        SetDisponible(conn, tx, p.libroId, false);

                        tx.Commit();
                        return nuevoId;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Devuelve un préstamo activo: registra la fecha de devolución,
        /// cambia el estado a 'Devuelto' y vuelve a poner el libro como disponible.
        /// </summary>
        public void Devolver(int prestamoId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        int libroId;
                        string estado;
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT libroId, estado FROM Prestamo WITH (UPDLOCK, ROWLOCK) WHERE id=@id", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", prestamoId);
                            using (SqlDataReader r = cmd.ExecuteReader())
                            {
                                if (!r.Read())
                                    throw new ReglaNegocioException("El préstamo no existe.");
                                libroId = (int)r["libroId"];
                                estado = r["estado"].ToString();
                            }
                        }

                        if (string.Equals(estado, "Devuelto", StringComparison.OrdinalIgnoreCase))
                            throw new ReglaNegocioException("El préstamo ya fue devuelto.");

                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE Prestamo SET fechaDevolucion=GETDATE(), estado='Devuelto' WHERE id=@id", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", prestamoId);
                            cmd.ExecuteNonQuery();
                        }

                        SetDisponible(conn, tx, libroId, true);

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        // DELETE (si el préstamo estaba activo, libera el libro)
        public bool Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        int libroId;
                        string estado;
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT libroId, estado FROM Prestamo WHERE id=@id", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            using (SqlDataReader r = cmd.ExecuteReader())
                            {
                                if (!r.Read()) { tx.Rollback(); return false; }
                                libroId = (int)r["libroId"];
                                estado = r["estado"].ToString();
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Prestamo WHERE id=@id", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }

                        if (!string.Equals(estado, "Devuelto", StringComparison.OrdinalIgnoreCase))
                            SetDisponible(conn, tx, libroId, true);

                        tx.Commit();
                        return true;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        // ---- Helpers ----

        private static bool Existe(SqlConnection conn, SqlTransaction tx, string tabla, int id)
        {
            using (SqlCommand cmd = new SqlCommand($"SELECT COUNT(1) FROM {tabla} WHERE id=@id", conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", id);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        // Devuelve null si el libro no existe; si existe, su disponibilidad. Bloquea la fila.
        private static bool? LibroDisponible(SqlConnection conn, SqlTransaction tx, int libroId)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT disponible FROM Libro WITH (UPDLOCK, ROWLOCK) WHERE id=@id", conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", libroId);
                object o = cmd.ExecuteScalar();
                if (o == null) return null;
                return (bool)o;
            }
        }

        private static void SetDisponible(SqlConnection conn, SqlTransaction tx, int libroId, bool disponible)
        {
            using (SqlCommand cmd = new SqlCommand("UPDATE Libro SET disponible=@d WHERE id=@id", conn, tx))
            {
                cmd.Parameters.AddWithValue("@d", disponible);
                cmd.Parameters.AddWithValue("@id", libroId);
                cmd.ExecuteNonQuery();
            }
        }

        private static Prestamo Map(SqlDataReader reader)
        {
            return new Prestamo
            {
                id = (int)reader["id"],
                libroId = (int)reader["libroId"],
                usuarioId = (int)reader["usuarioId"],
                fechaPrestamo = reader["fechaPrestamo"].ToString(),
                fechaLimite = reader["fechaLimite"].ToString(),
                fechaDevolucion = reader["fechaDevolucion"] == DBNull.Value ? null : reader["fechaDevolucion"].ToString(),
                estado = reader["estado"].ToString()
            };
        }
    }
}
