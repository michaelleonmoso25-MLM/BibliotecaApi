using BibliotecaApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BibliotecaApi.Data
{
    public class PrestamoDAO
    {
        string connectionString =
            ConfigurationManager.ConnectionStrings["BibliotecaConnection"].ConnectionString;

        // GET ALL
        public List<Prestamo> GetAll()
        {
            List<Prestamo> lista = new List<Prestamo>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Prestamo";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new Prestamo()
                    {
                        id = (int)reader["id"],
                        libroId = (int)reader["libroId"],
                        usuarioId = (int)reader["usuarioId"],
                        fechaPrestamo = reader["fechaPrestamo"].ToString(),
                        fechaLimite = reader["fechaLimite"].ToString(),
                        fechaDevolucion = reader["fechaDevolucion"].ToString(),
                        estado = reader["estado"].ToString()
                    });
                }
            }
            return lista;
        }

        // GET BY ID
        public Prestamo GetById(int id)
        {
            Prestamo p = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Prestamo WHERE id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    p = new Prestamo()
                    {
                        id = (int)reader["id"],
                        libroId = (int)reader["libroId"],
                        usuarioId = (int)reader["usuarioId"],
                        fechaPrestamo = reader["fechaPrestamo"].ToString(),
                        fechaLimite = reader["fechaLimite"].ToString(),
                        fechaDevolucion = reader["fechaDevolucion"].ToString(),
                        estado = reader["estado"].ToString()
                    };
                }
            }
            return p;
        }

        // INSERT
        public void Insert(Prestamo p)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query =
                    "INSERT INTO Prestamo(libroId,usuarioId,fechaPrestamo,fechaLimite,fechaDevolucion,estado) VALUES(@libroId,@usuarioId,@fechaPrestamo,@fechaLimite,@fechaDevolucion,@estado)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@libroId", p.libroId);
                cmd.Parameters.AddWithValue("@usuarioId", p.usuarioId);
                cmd.Parameters.AddWithValue("@fechaPrestamo", p.fechaPrestamo);
                cmd.Parameters.AddWithValue("@fechaLimite", p.fechaLimite);
                cmd.Parameters.AddWithValue("@fechaDevolucion", (object)p.fechaDevolucion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@estado", p.estado);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // UPDATE
        public void Update(int id, Prestamo p)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Prestamo
                    SET libroId=@libroId, usuarioId=@usuarioId, fechaPrestamo=@fechaPrestamo, fechaLimite=@fechaLimite, fechaDevolucion=@fechaDevolucion, estado=@estado
                    WHERE id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@libroId", p.libroId);
                cmd.Parameters.AddWithValue("@usuarioId", p.usuarioId);
                cmd.Parameters.AddWithValue("@fechaPrestamo", p.fechaPrestamo);
                cmd.Parameters.AddWithValue("@fechaLimite", p.fechaLimite);
                cmd.Parameters.AddWithValue("@fechaDevolucion", (object)p.fechaDevolucion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@estado", p.estado);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // DELETE
        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Prestamo WHERE id=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}