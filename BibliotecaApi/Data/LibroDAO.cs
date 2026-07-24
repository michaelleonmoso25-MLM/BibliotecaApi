using BibliotecaApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BibliotecaApi.Data
{
    public class LibroDAO
    {
        string connectionString =
            ConfigurationManager.ConnectionStrings["BibliotecaConnection"].ConnectionString;

        // GET ALL
        public List<Libro> GetAll()
        {
            List<Libro> lista = new List<Libro>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Libro";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new Libro()
                    {
                        id = (int)reader["id"],
                        titulo = reader["titulo"].ToString(),
                        autor = reader["autor"].ToString(),
                        isbn = reader["isbn"].ToString(),
                        categoria = reader["categoria"].ToString(),
                        portadaUrl = reader["portadaUrl"].ToString(),
                        disponible = (bool)reader["disponible"]
                    });
                }
            }
            return lista;
        }

        // GET BY ID
        public Libro GetById(int id)
        {
            Libro l = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Libro WHERE id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    l = new Libro()
                    {
                        id = (int)reader["id"],
                        titulo = reader["titulo"].ToString(),
                        autor = reader["autor"].ToString(),
                        isbn = reader["isbn"].ToString(),
                        categoria = reader["categoria"].ToString(),
                        portadaUrl = reader["portadaUrl"].ToString(),
                        disponible = (bool)reader["disponible"]
                    };
                }
            }
            return l;
        }

        // INSERT
        public void Insert(Libro l)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query =
                    "INSERT INTO Libro(titulo,autor,isbn,categoria,portadaUrl,disponible) VALUES(@titulo,@autor,@isbn,@categoria,@portadaUrl,@disponible)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@titulo", l.titulo);
                cmd.Parameters.AddWithValue("@autor", l.autor);
                cmd.Parameters.AddWithValue("@isbn", l.isbn);
                cmd.Parameters.AddWithValue("@categoria", l.categoria);
                cmd.Parameters.AddWithValue("@portadaUrl", l.portadaUrl);
                cmd.Parameters.AddWithValue("@disponible", l.disponible);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // UPDATE
        public void Update(int id, Libro l)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Libro
                    SET titulo=@titulo, autor=@autor, isbn=@isbn, categoria=@categoria, portadaUrl=@portadaUrl, disponible=@disponible
                    WHERE id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@titulo", l.titulo);
                cmd.Parameters.AddWithValue("@autor", l.autor);
                cmd.Parameters.AddWithValue("@isbn", l.isbn);
                cmd.Parameters.AddWithValue("@categoria", l.categoria);
                cmd.Parameters.AddWithValue("@portadaUrl", l.portadaUrl);
                cmd.Parameters.AddWithValue("@disponible", l.disponible);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // DELETE
        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Libro WHERE id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}