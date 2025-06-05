namespace NetCoreIoT
{
    public class ApiResponse<T>
    {
        public long Code { get; set; } = 0;  // 0 = 成功，其他为错误码
        public string Message { get; set; } = "成功";
        public T? Data { get; set; }

        public static ApiResponse<T> Success(T data, string? msg = null)
            => new ApiResponse<T> { Code = 0, Message = msg ?? "成功", Data = data };

        public static ApiResponse<T> Fail(string message, int code = 1)
            => new ApiResponse<T> { Code = code, Message = message, Data = default };
    }

}
