namespace Tienda.API.DTOs
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        // 🔹 Constructor éxito
        public ApiResponse(T data, string message = "Operación exitosa")
        {
            Data = data;
            Success = true;
            Message = message;
        }

        // 🔹 Constructor error de negocio (IMPORTANTE)
        public ApiResponse(string message)
        {
            Data = default;
            Success = false;
            Message = message;
        }

        // Constructor vacío
        public ApiResponse() { }
    }
}