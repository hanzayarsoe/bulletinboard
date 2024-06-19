namespace MTM.Entities.DTO
{
    public class ResponseModel
    {
        public ResponseModel() 
        {
            this.ResponseType = 0;
            this.ResponseMessage = string.Empty;
            this.Data = new Dictionary<string, string>();
        }

        #region Properties
        public int ResponseType { get; set; }
        public string ResponseMessage { get; set; }
        public Dictionary<string, string> Data { get; set; }
        #endregion
    }
}
