using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing.Text;

namespace AdoNetConectado
{
    public partial class frmTipoCliente : Form
    {
        string cadenaConexion = @"server=localhost\SQLEXPRESS;DataBase=BancoBD; Integrated Security=true;";

        public frmTipoCliente()
        {
            InitializeComponent();                                
        }

        private void cargarFormulario(object sender, EventArgs e)
        {
            cargarDatos();
        }
        private void cargarDatos()
        {
            //using delimita el ciclo ddde vida de una variable
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqlCommand("Select * from TipoCliente", conexion))
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector != null && lector.HasRows)
                        {
                            while (lector.Read())
                            {
                                dgvDatos.Rows.Add(lector[0], lector[1], lector[2], lector[3]);
                            }
                        }
                    }
                }
            }


            //=====Metodo Simple de conexion=====
            //
            //var conexion = new SqlConnection(cadenaConexion);
            //conexion.Open();
            //var querySql = "SELECT * FROM TipoCliente";
            //var comando = new SqlCommand(querySql, conexion);
            //var reader = comando.ExecuteReader();
            //if(reader != null && reader.HasRows)
            //{
            //    reader.Read();
            //    dgvDatos.Rows.Add(reader[0], reader[1]);                             
            //}
            //conexion.Close();
        }

        private void nuevoRegistro(object sender, EventArgs e)
        {
            var frm = new frmTipoClienteEdit();
            if(frm.ShowDialog() == DialogResult.OK)
            {
                string nombre = frm.Controls["txtNombre"].Text;
                string descripcion = frm.Controls["txtDescripcion"].Text;
                //operador ternario
                int estado = ((CheckBox)frm.Controls["chkEstado"]).Checked == true ? 1: 0;

                using (var conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    using (var comando = new SqlCommand("insert into TipoCliente (Nombre, Descripcion, Estado)" +
                        "values (@nombre, @descripcion, @estado) ", conexion))
                    {
                        comando.Parameters.AddWithValue("@nombre", nombre);
                        comando.Parameters.AddWithValue("@descripcion", descripcion);
                        comando.Parameters.AddWithValue("@estado", estado);
                        int resultado = comando.ExecuteNonQuery();
                        if(resultado > 0)
                        {
                            MessageBox.Show("Datos registrados", "Sistemas",
                                MessageBoxButtons.OK,MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No se ha podido registrar los datos.", "Sistemas",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                
                //caso combobox
                //string dato = (combobox)frm.Controls["cbolista"].SelectedValue.ToString();
            }
        }
        private void editarRegistro(object sender, EventArgs e)
        {           
            //VALIDAMOS QUE EXISTE FILAS PARA EDITAR
            if (dgvDatos.RowCount > 0 && dgvDatos.CurrentRow != null)
            {
                //TOMAMOS EL ID DE LA FILA SELECCIONADA
                int idtipo = int.Parse(dgvDatos.CurrentRow.Cells[0].Value.ToString());
                var frm = new frmTipoClienteEdit(idtipo);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    string nombre = frm.Controls["txtNombre"].Text;
                    string descripcion = frm.Controls["txtDescripcion"].Text;
                    int estado = ((CheckBox)frm.Controls["chkEstado"]).Checked == true ? 1 : 0;
                    using (var conexion = new SqlConnection(cadenaConexion))
                    {
                        conexion.Open();
                        using(var comando = new SqlCommand("update TipoCliente set Nombre = @nombre," +
                            "Descripcion = @descripcion, Estado = @estado where ID = @id", conexion))
                        {
                            comando.Parameters.AddWithValue("@nombre", nombre);
                            comando.Parameters.AddWithValue("@descripcion", descripcion);
                            comando.Parameters.AddWithValue("@estado", estado);
                            comando.Parameters.AddWithValue("@id", idtipo);
                            int resultado = comando.ExecuteNonQuery();
                            if (resultado > 0)
                            {
                                MessageBox.Show("Datos actualizados", "Sistemas",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("No se ha podido actualizar los datos.", "Sistemas",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }

            }
        }

        private void eliminarRegistro(object sender, EventArgs e)
        {
            if (dgvDatos.RowCount > 0 && dgvDatos.CurrentRow != null)
            {               
                int idtipo = int.Parse(dgvDatos.CurrentRow.Cells[0].Value.ToString());
                var frm = new frmTipoClienteEdit(idtipo);
                
                    using (var conexion = new SqlConnection(cadenaConexion))
                    {
                        conexion.Open();
                        using (var comando = new SqlCommand("Delete from TipoCliente where ID = @id", conexion))
                        {
                            comando.Parameters.AddWithValue("@id", idtipo);
                            int resultado = comando.ExecuteNonQuery();
                            if (resultado > 0)
                            {
                                MessageBox.Show("Datos Eliminados", "Sistemas",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("No se ha podido Eliminar los datos.", "Sistemas",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                
            }
            
        }
    }
}
