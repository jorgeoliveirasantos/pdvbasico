using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PdvBasico
{
    public partial class MainWindow : Window
    {
        // Classes simples para os dados
        public class Produto
        {
            public int Codigo { get; set; }
            public string Nome { get; set; }
            public decimal Preco { get; set; }
            public int Estoque { get; set; }
        }

        public class Cliente
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public string Telefone { get; set; }
            public string Email { get; set; }
        }

        public class ItemVenda
        {
            public int ProdutoCodigo { get; set; }
            public string ProdutoNome { get; set; }
            public int Quantidade { get; set; }
            public decimal PrecoUnitario { get; set; }
            public decimal Subtotal => Quantidade * PrecoUnitario;
        }

        // Listas para armazenar dados (simulando banco de dados)
        private List<Produto> produtos = new List<Produto>();
        private List<Cliente> clientes = new List<Cliente>();
        private List<ItemVenda> itensVenda = new List<ItemVenda>();

        // Contadores simples
        private int proximoIdCliente = 1;
        private int proximoCodigoProduto = 100;

        public MainWindow()
        {
            InitializeComponent();
            
            // Carregar dados de exemplo
            CarregarDadosExemplo();
            
            // Inicializar componentes
            InicializarVenda();
            
            // Mostrar status inicial
            AtualizarStatus("Sistema iniciado. Pronto para vender!", "#28a745");
        }

        private void CarregarDadosExemplo()
        {
            // Adicionar produtos de exemplo
            produtos.Add(new Produto { 
                Codigo = proximoCodigoProduto++, 
                Nome = "Notebook Dell Inspiron", 
                Preco = 3299.90m, 
                Estoque = 10 
            });
            produtos.Add(new Produto { 
                Codigo = proximoCodigoProduto++, 
                Nome = "Mouse Wireless Logitech", 
                Preco = 89.90m, 
                Estoque = 50 
            });
            produtos.Add(new Produto { 
                Codigo = proximoCodigoProduto++, 
                Nome = "Teclado Mecânico Razer", 
                Preco = 299.90m, 
                Estoque = 15 
            });
            produtos.Add(new Produto { 
                Codigo = proximoCodigoProduto++, 
                Nome = "Monitor 24\" Samsung", 
                Preco = 899.90m, 
                Estoque = 8 
            });
            produtos.Add(new Produto { 
                Codigo = proximoCodigoProduto++, 
                Nome = "Cabo HDMI 2.0", 
                Preco = 29.90m, 
                Estoque = 100 
            });

            // Adicionar clientes de exemplo
            clientes.Add(new Cliente { 
                Id = proximoIdCliente++, 
                Nome = "João Silva", 
                Telefone = "(11) 99999-9999", 
                Email = "joao@email.com" 
            });
            clientes.Add(new Cliente { 
                Id = proximoIdCliente++, 
                Nome = "Maria Santos", 
                Telefone = "(11) 98888-8888", 
                Email = "maria@email.com" 
            });

            // Atualizar DataGrid de produtos
            dgProdutos.ItemsSource = null;
            dgProdutos.ItemsSource = produtos;
        }

        private void InicializarVenda()
        {
            itensVenda.Clear();
            dgItensVenda.ItemsSource = null;
            dgItensVenda.ItemsSource = itensVenda;
            AtualizarResumoVenda();
        }

        private void BtnCadastrarCliente_Click(object sender, RoutedEventArgs e)
        {
            string nome = txtClienteNome.Text.Trim();
            string telefone = txtClienteTelefone.Text.Trim();
            string email = txtClienteEmail.Text.Trim();

            // Validação simples
            if (string.IsNullOrWhiteSpace(nome))
            {
                MessageBox.Show("Por favor, informe o nome do cliente.", "Atenção", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Criar novo cliente
            Cliente novoCliente = new Cliente
            {
                Id = proximoIdCliente++,
                Nome = nome,
                Telefone = telefone,
                Email = email
            };

            // Adicionar à lista
            clientes.Add(novoCliente);

            // Limpar campos
            txtClienteNome.Clear();
            txtClienteTelefone.Clear();
            txtClienteEmail.Clear();

            // Mostrar mensagem de sucesso
            AtualizarStatus($"Cliente '{nome}' cadastrado com sucesso! (ID: {novoCliente.Id})", "#28a745");
            
            MessageBox.Show($"Cliente cadastrado com sucesso!\nID: {novoCliente.Id}\nNome: {nome}", 
                "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnAdicionarProduto_Click(object sender, RoutedEventArgs e)
        {
            if (dgProdutos.SelectedItem is Produto produtoSelecionado)
            {
                // Verificar se produto já está na venda
                ItemVenda itemExistente = itensVenda.FirstOrDefault(i => i.ProdutoCodigo == produtoSelecionado.Codigo);
                
                if (itemExistente != null)
                {
                    // Aumentar quantidade
                    itemExistente.Quantidade++;
                }
                else
                {
                    // Adicionar novo item
                    ItemVenda novoItem = new ItemVenda
                    {
                        ProdutoCodigo = produtoSelecionado.Codigo,
                        ProdutoNome = produtoSelecionado.Nome,
                        Quantidade = 1,
                        PrecoUnitario = produtoSelecionado.Preco
                    };
                    itensVenda.Add(novoItem);
                }

                // Atualizar display
                dgItensVenda.ItemsSource = null;
                dgItensVenda.ItemsSource = itensVenda;
                AtualizarResumoVenda();
                
                AtualizarStatus($"'{produtoSelecionado.Nome}' adicionado à venda", "#17a2b8");
            }
            else
            {
                MessageBox.Show("Selecione um produto na lista para adicionar à venda.", "Atenção", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AtualizarResumoVenda()
        {
            int totalItens = itensVenda.Sum(i => i.Quantidade);
            decimal valorTotal = itensVenda.Sum(i => i.Subtotal);

            txtTotalItens.Text = totalItens.ToString();
            txtValorTotal.Text = valorTotal.ToString("C2");
        }

        private void BtnFinalizarVenda_Click(object sender, RoutedEventArgs e)
        {
            if (!itensVenda.Any())
            {
                MessageBox.Show("Adicione pelo menos um item à venda antes de finalizar.", "Atenção", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbFormaPagamento.SelectedItem == null)
            {
                MessageBox.Show("Selecione uma forma de pagamento.", "Atenção", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string formaPagamento = ((ComboBoxItem)cbFormaPagamento.SelectedItem).Content.ToString();
            decimal valorTotal = itensVenda.Sum(i => i.Subtotal);
            int totalItens = itensVenda.Sum(i => i.Quantidade);

            // Aqui normalmente salvaríamos no "banco de dados"
            // Para simplificar, vamos apenas mostrar um resumo
            
            string mensagem = $"VENDA FINALIZADA COM SUCESSO!\n\n" +
                             $"Itens Vendidos: {totalItens}\n" +
                             $"Valor Total: {valorTotal:C2}\n" +
                             $"Forma de Pagamento: {formaPagamento}\n\n" +
                             $"Obrigado pela preferência!";
            
            MessageBox.Show(mensagem, "Venda Concluída", 
                MessageBoxButton.OK, MessageBoxImage.Information);

            // Iniciar nova venda
            InicializarVenda();
            cbFormaPagamento.SelectedIndex = -1;
            
            AtualizarStatus("Venda finalizada! Pronta para nova venda.", "#28a745");
        }

        private void BtnCancelarVenda_Click(object sender, RoutedEventArgs e)
        {
            if (itensVenda.Any())
            {
                MessageBoxResult result = MessageBox.Show(
                    "Tem certeza que deseja cancelar a venda? Todos os itens serão removidos.",
                    "Cancelar Venda",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    InicializarVenda();
                    cbFormaPagamento.SelectedIndex = -1;
                    AtualizarStatus("Venda cancelada. Pronta para nova venda.", "#ffc107");
                }
            }
            else
            {
                MessageBox.Show("Não há itens na venda para cancelar.", "Informação", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AtualizarStatus(string mensagem, string corHex)
        {
            txtStatus.Text = mensagem;
            txtStatus.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter()
                .ConvertFromString(corHex);
        }

        // Método adicional para demonstração: adicionar produto personalizado
        public void AdicionarNovoProduto(string nome, decimal preco, int estoque)
        {
            Produto novoProduto = new Produto
            {
                Codigo = proximoCodigoProduto++,
                Nome = nome,
                Preco = preco,
                Estoque = estoque
            };
            
            produtos.Add(novoProduto);
            dgProdutos.ItemsSource = null;
            dgProdutos.ItemsSource = produtos;
            
            AtualizarStatus($"Produto '{nome}' adicionado ao sistema", "#28a745");
        }
    }
}