// ferreteria_je\Repositories\Interfaces\IUsuarioRepository.cs
using ferreteria_je.Repositories.Models; // Asegura que apunta al namespace correcto del modelo
using ferreteria_je.Repositories.RepositoriesGeneric.Interfaces; // Asegura que apunta a IRepository
using System.Collections.Generic;

namespace ferreteria_je.Repositories.Interfaces
{
    public interface IUsuarioRepository : IRepository<usuario> // <-- ¡AHORA ES 'usuario' (singular)!
    {
        usuario GetByEmail(string email); // <-- ¡AHORA ES 'usuario' (singular)!
        List<usuario> GetUsuariosByNombre(string nombreParcial); // <-- ¡AHORA ES 'usuario' (singular)!
    }
}