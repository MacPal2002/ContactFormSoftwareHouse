using Projekt001.Entities;

namespace Projekt001.Repositories // Changed namespace for better organization
{
    public interface IContactFormRepository
    {
        /// <summary>
        /// Adds a new contact entity to the data store.
        /// </summary>
        /// <param name="contactEntity">The contact entity to add.</param>
        Task AddAsync(ContactEntity contactEntity);

        /// <summary>
        /// Retrieves all contact entities from the data store.
        /// (This might not be used by the contact form itself, but good for admin purposes)
        /// </summary>
        /// <returns>A list of contact entities.</returns>
        Task<List<ContactEntity>> GetAllAsync();

        /// <summary>
        /// Retrieves a contact entity by its ID.
        /// (This might not be used by the contact form itself)
        /// </summary>
        /// <param name="id">The ID of the contact entity.</param>
        /// <returns>The contact entity if found; otherwise, null.</returns>
        Task<ContactEntity?> GetByIdAsync(int id);

        /// <summary>
        /// Saves all changes made in the context to the underlying database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync();
    }
}