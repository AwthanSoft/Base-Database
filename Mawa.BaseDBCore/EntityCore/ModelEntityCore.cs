using System;
using System.ComponentModel.DataAnnotations;

namespace Mawa.BaseDBCore.EntityCore
{
    public abstract class ModelEntityCore : IModelEntityCore
    {
        public ModelEntityCore()
        {
            this.ObjectId = Mawa.Randoms.RandomId.Object_Id();
            this.CreateDate = DateTime.Now;
            this.ModifiedDate = DateTime.Now;
        }

        [Required]
        [Display(AutoGenerateField = false)]
        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "VARCHAR(20)")]
        public string ObjectId { get; set; }

        [Required]
        [System.ComponentModel.DisplayName("Created")]
        //[System.ComponentModel.DataAnnotations.Timestamp]
        //[System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Computed)]
        public System.DateTime CreateDate { get; set; }

        [Required]
        [System.ComponentModel.DisplayName("Modified")]
        //[System.ComponentModel.DataAnnotations.Timestamp]
        //[System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Computed)]
        public System.DateTime ModifiedDate { get; set; }

    }
}
