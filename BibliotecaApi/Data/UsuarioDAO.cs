using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using BibliotecaApi.Models;
using BibliotecaApi.Security;

namespace BibliotecaApi.Data
{
    public class UsuarioDAO
    {
        string connectionString =
            ConfigurationManager.ConnectionStrings["BibliotecaConnection"].ConnectionString;

        // GET ALL (nunca devuelve la contraseña)
        public List<Usuario> GetAll()
        {
            List<Usuario> lista = new List<Usuario>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT id, nombre, correo, rol FROM Usuario";
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

        // GET BY ID (sin contraseña)
        public Usuario GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT id, nombre, correo, rol FROM Usuario WHERE id=@id";
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

        // Usado solo por el login: incluye el hash de la contraseña.
        public Usuario GetByCorreoConPassword(string correo)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT id, nombre, correo, password, rol FROM Usuario WHERE correo=@correo";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@correo", correo);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var u = Map(reader);
                            u.password = reader["password"].ToString();
                            return u;
                        }
                    }
                }
            }
            return null;
        }

        public bool CorreoExiste(string correo)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM Usuario WHERE correo=@correo";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@correo", correo);
                    conn.Open();
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        // INSERT (hashea la contraseña). Devuelve el id generado.
        public int Insert(Usuario u)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query =
                    "INSERT INTO Usuario(nombre, correo, password, rol) " +
                    "VALUES(@nombre, @correo, @password, @rol); SELECT CAST(SCOPE_IDENTITY() AS int);";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", u.nombre);
                    cmd.Parameters.AddWithValue("@correo", u.correo);
                    cmd.Parameters.AddWithValue("@password", PasswordHelper.Hash(u.password));
                    cmd.Parameters.AddWithValue("@rol", u.rol);
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        // UPDATE (re-hashea la contraseña solo si se envía una nueva). Devuelve filas afectadas.
        public int Update(int id, Usuario u)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                bool cambiaPassword = !string.IsNullOrEmpty(u.password);
                string query = cambiaPassword
                    ? "UPDATE Usuario SET nombre=@nombre, correo=@correo, password=@password, rol=@rol WHERE id=@id"
                    : "UPDATE Usuario SET nombre=@nombre, correo=@correo, rol=@rol WHERE id=@id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@nombre", u.nombre);
                    cmd.Parameters.AddWithValue("@correo", u.correo);
                    cmd.Parameters.AddWithValue("@rol", u.rol);
                    if (cambiaPassword)
                        cmd.Parameters.AddWithValue("@password", PasswordHelper.Hash(u.password));
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public int Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Usuario WHERE id=@id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        private static Usuario Map(SqlDataReader reader)
        {
            return new Usuario
            {
                id = (int)reader["id"],
                nombre = reader["nombre"].ToString(),
                correo = reader["correo"].ToString(),
                rol = reader["rol"].ToString()
            };
        }
    }
}
