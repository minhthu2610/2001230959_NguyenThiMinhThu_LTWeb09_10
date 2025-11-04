namespace LTWeb09_BTM1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SACH")]
    public partial class SACH
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SACH()
        {
            CHITIETDONHANGs = new HashSet<CHITIETDONHANG>();
            THAMGIAs = new HashSet<THAMGIA>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MASACH { get; set; }

        [StringLength(100)]
        public string TENSACH { get; set; }

        public decimal? GIABAN { get; set; }

        [Column(TypeName = "ntext")]
        public string MOTA { get; set; }

        [StringLength(100)]
        public string ANHBIA { get; set; }

        public DateTime? NGAYCAPNHAT { get; set; }

        public int? SOLUONGBAN { get; set; }

        public int? MACD { get; set; }

        public int? MANXB { get; set; }

        public bool? MOI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHITIETDONHANG> CHITIETDONHANGs { get; set; }

        public virtual CHUDE CHUDE { get; set; }

        public virtual NHAXUATBAN NHAXUATBAN { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<THAMGIA> THAMGIAs { get; set; }
    }
}
