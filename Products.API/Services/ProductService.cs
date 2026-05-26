using ECommerce.Shared.Exceptions; // IMPORTANTE: Usa las alarmas de Leandro
using Products.API.Models;
using System.Threading.Tasks;

namespace Products.API.Services
{
    // Definimos el contrato del servicio
    public interface IProductService
    {
        Task<Product> CreateProductAsync(Product product);
    }

    // Implementamos el servicio
    public class ProductService : IProductService
    {
        public async Task<Product> CreateProductAsync(Product product)
        {
            // SIMULACIÓN: Verificar si ya existe en la base de datos (Para el re)
            bool productoExiste = await VerificarSiExiste(product.Nombre, product.Categoria);

            if (productoExiste)
            {
                // Dispara el error PRD-003. El "guardia" de Leandro lo atrapará
                throw new BusinessRuleException(
                    "PRD-003",
                    $"Ya existe un producto con ese nombre en la categoría '{product.Categoria}'."
                );
            }

            // TODO: Guardar producto usando la persistencia de la cátedra
            // await _repository.SaveAsync(product);

            return product;
        }

        // Método auxiliar simulado para probar el error
        private Task<bool> VerificarSiExiste(string nombre, string categoria)
        {
            // Para probar que el error 409 funcione, podés cambiar false a true
            return Task.FromResult(false);
        }
    }
}