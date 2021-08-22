using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DBAppCore.EntityCore
{
    public abstract class ModelEntityCore : DBModelCore
    {
        public ModelEntityCore()
        {
            this.ObjectId = CommonAppCore.Randoms.RandomId.Object_Id();
            this.CreateDate = DateTime.Now;
            this.ModifiedDate = DateTime.Now;
        }

        //[Required]
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
