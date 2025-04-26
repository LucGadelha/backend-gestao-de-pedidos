using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoDesPedidosApi.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public string Cliente { get; set; }
        
        [Required]
        public string Produto { get; set; }
        
        [Required]
        public decimal Valor { get; set; }
        
        [Required]
        public string Status { get; set; }
        
        public DateTime DataCriacao { get; set; }
        
        public Order()
        {
            Id = Guid.NewGuid();
            Status = "Pendente";
            DataCriacao = DateTime.Now;
        }
    }
}
