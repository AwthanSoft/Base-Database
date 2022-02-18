namespace Mawa.BaseDBCore.ViewEntityCore
{
    public abstract class ModelViewEntityCore : IModelViewEntityCore
    {
        //[Key]
        //[System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public string ObjectId => GetObjectId;
        protected abstract string GetObjectId { get; }
    }
}
