using sm10.Modelo;
using System;
using System.Linq;
using System.Windows.Forms;

namespace sm10
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FiltrarClientes();
            txtCliente.TextChanged += new EventHandler(txtCliente_TextChanged);
            dateTimePickerDesde.ValueChanged += new EventHandler(ActualizarOrdenesDeCompra);
            dateTimePickerHasta.ValueChanged += new EventHandler(ActualizarOrdenesDeCompra);
            dataGridViewClientes.SelectionChanged += new EventHandler(dataGridViewClientes_SelectionChanged);
        }

        private void FiltrarClientes()
        {
            string textoFiltro = txtCliente.Text.Trim().ToLower();
            using (var BD = new NegociosEntities())
            {
                var clientesFiltrados = from cliente in BD.CLIENTE
                                        where cliente.NombreCia.ToLower().Contains(textoFiltro)
                                        select new
                                        {
                                            cliente.IdCliente,
                                            cliente.NombreCia,
                                            cliente.Direccion,
                                            cliente.Telefono,
                                            Pais = cliente.PAIS.NombrePais 
                                        };

                dataGridViewClientes.DataSource = clientesFiltrados.ToList();
            }
        }
        private void ActualizarOrdenesDeCompra(object sender, EventArgs e)
        {
            if (dataGridViewClientes.SelectedRows.Count > 0)
            {
                string clienteId = dataGridViewClientes.SelectedRows[0].Cells["IdCliente"].Value.ToString();
                DateTime fechaDesde = dateTimePickerDesde.Value.Date;
                DateTime fechaHasta = dateTimePickerHasta.Value.Date;

                using (var BD = new NegociosEntities())
                {
                    var ordenes = from pedido in BD.PEDIDO
                                  where pedido.IdCliente == clienteId &&
                                        pedido.FechaPedido >= fechaDesde &&
                                        pedido.FechaPedido <= fechaHasta
                                  orderby pedido.FechaPedido
                                  select new
                                  {
                                      IdPedido = pedido.IdPedido,
                                      Cliente = pedido.CLIENTE.NombreCia,
                                      Fecha = pedido.FechaPedido,
                                      Total = pedido.DETALLEPEDIDO.Sum(d => d.Cantidad * d.PrecioUnidad)
                                  };

                    dataGridViewOrdenes.DataSource = ordenes.ToList();
                }
            }
            else
            {
                dataGridViewOrdenes.DataSource = null;
            }
        }


        private void FiltrarOrdenesDeCompra()
        {
            if (dataGridViewClientes.SelectedRows.Count > 0)
            {
                string clienteId = dataGridViewClientes.SelectedRows[0].Cells["IdCliente"].Value.ToString();
                DateTime fechaDesde = dateTimePickerDesde.Value.Date;
                DateTime fechaHasta = dateTimePickerHasta.Value.Date;

                using (var BD = new NegociosEntities())
                {
                    var ordenes = from pedido in BD.PEDIDO
                                  where pedido.IdCliente == clienteId &&
                                        pedido.FechaPedido >= fechaDesde &&
                                        pedido.FechaPedido <= fechaHasta
                                  orderby pedido.FechaPedido
                                  select new
                                  {
                                      IdPedido = pedido.IdPedido,
                                      Cliente = pedido.CLIENTE.NombreCia,
                                      Fecha = pedido.FechaPedido,
                                      Total = pedido.DETALLEPEDIDO.Sum(d => d.Cantidad * d.PrecioUnidad)
                                  };

                    dataGridViewOrdenes.DataSource = ordenes.ToList();
                }
            }
            else
            {
                dataGridViewOrdenes.DataSource = null;
            }
        }

        private void txtCliente_TextChanged(object sender, EventArgs e)
        {
            FiltrarClientes();
        }

        private void dataGridViewClientes_SelectionChanged(object sender, EventArgs e)
        {
            FiltrarOrdenesDeCompra();
        }

        private void dateTimePickerDesde_ValueChanged(object sender, EventArgs e)
        {
            FiltrarOrdenesDeCompra();
        }

        private void dateTimePickerHasta_ValueChanged(object sender, EventArgs e)
        {
            FiltrarOrdenesDeCompra();
        }

        private void dataGridViewClientes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                FiltrarOrdenesDeCompra();
            }
        }

        private void dateTimePickerDesde_KeyDown(object sender, KeyEventArgs e)
        {
            FiltrarOrdenesDeCompra();
        }

        private void dateTimePickerHasta_KeyDown(object sender, KeyEventArgs e)
        {
            FiltrarOrdenesDeCompra();
        }

        private void dataGridViewOrdenes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            FiltrarOrdenesDeCompra();
        }
    }
}
