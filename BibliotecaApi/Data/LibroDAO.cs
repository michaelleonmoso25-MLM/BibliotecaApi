using BibliotecaApi.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace BibliotecaApi.Data
{
    public class LibroDAO
    {
        string connectionString =
            ConfigurationManager.ConnectionStrings["BibliotecaConnection"].ConnectionString;

        private const string Columnas = "id, titulo, autor, isbn, categoria, portadaUrl, disponible";

        // GET ALL
        public List<Libro> GetAll()
        {
            List<Libro> lista = new List<Libro>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand($"SELECT {Columnas} FROM Libro", conn))
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
        public Libro GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand($"SELECT {Columnas} FROM Libro WHERE id=@id", conn))
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

        // INSERT (devuelve el id generado)
        public int Insert(Libro l)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query =
                    "INSERT INTO Libro(titulo, autor, isbn, categoria, portadaUrl, disponible) " +
                    "VALUES(@titulo, @autor, @isbn, @categoria, @portadaUrl, @disponible); " +
                    "SELECT CAST(SCOPE_IDENTITY() AS int);";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    AgregarParametros(cmd, l);
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        // UPDATE (devuelve filas afectadas)
        public int Update(int id, Libro l)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query =
                    "UPDATE Libro SET titulo=@titulo, autor=@autor, isbn=@isbn, " +
                    "categoria=@categoria, portadaUrl=@portadaUrl, disponible=@disponible WHERE id=@id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    AgregarParametros(cmd, l);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // DELETE (devuelve filas afectadas)
        public int Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Libro WHERE id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        private static void AgregarParametros(SqlCommand cmd, Libro l)
        {
            cmd.Parameters.AddWithValue("@titulo", l.titulo);
            cmd.Parameters.AddWithValue("@autor", (object)l.autor ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@isbn", (object)l.isbn ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@categoria", (object)l.categoria ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@portadaUrl", (object)l.portadaUrl ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@disponible", l.disponible);
        }

        private static Libro Map(SqlDataReader reader)
        {
            return new Libro
            {
                id = (int)reader["id"],
                titulo = reader["titulo"].ToString(),
                autor = reader["autor"] == System.DBNull.Value ? null : reader["autor"].ToString(),
                isbn = reader["isbn"] == System.DBNull.Value ? null : reader["isbn"].ToString(),
                categoria = reader["categoria"] == System.DBNull.Value ? null : reader["categoria"].ToString(),
                portadaUrl = reader["portadaUrl"] == System.DBNull.Value ? null : reader["portadaUrl"].ToString(),
                disponible = (bool)reader["disponible"]
            };
        }
    }
}
