namespace Core.IServices
{
    public interface I_W_Service<T>
    {
        /// <summary>
        /// Sends one object for storage
        /// </summary>
        /// <param name="obj">Object to store</param>
        /// <returns>Specified object, eventually updated with auto-generated ID</returns>
        public T Store(T obj);

        /// <summary>
        /// Sends one object for updating in storage
        /// </summary>
        /// <param name="obj">Object to update</param>
        /// <returns>Updated object</returns>
        public T Update(T obj);

        /// <summary>
        /// Sends one object for deletion
        /// </summary>
        /// <param name="obj">Object to delete</param>
        /// <returns>Deleted object</returns>
        public T Delete(T obj);

        public bool Validate(T obj);
    }
}