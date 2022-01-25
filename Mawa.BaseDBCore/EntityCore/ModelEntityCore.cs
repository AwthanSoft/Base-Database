using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "VARCHAR(20)")]
        public string ObjectId { get; set; }

        [Required]
        //[System.ComponentModel.DataAnnotations.Timestamp]
        //[System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Computed)]
        public System.DateTime CreateDate { get; set; }

        [Required]
        //[System.ComponentModel.DataAnnotations.Timestamp]
        //[System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Computed)]
        public System.DateTime ModifiedDate { get; set; }

    }
}
