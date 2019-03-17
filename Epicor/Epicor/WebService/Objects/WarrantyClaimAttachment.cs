using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epicor.WebService.AttachmentService;
using Epicor.AttachmentServiceReference.Ice;

namespace Epicor.Objects
{
    public class WarrantyClaimAttachment
    {
        #region Properties
        public int AttachmentID { get; private set; }
        public string LeeBoyID { get; private set; }
        public string Url { get; private set; }
        public string Description { get; private set; }
        #endregion

        private WarrantyClaimAttachment()
        {

        }

        public WarrantyClaimAttachment(string leeBoyId, int attachmentId)
        {
            List<WarrantyClaimAttachment> attachments = WarrantyClaimAttachmentsForWarrantyClaim(leeBoyId);

            if (attachments == null || attachments.Count() <= 0)
            {
                throw new ArgumentOutOfRangeException("There are no matching attachments for the claim with LeeBoy ID " + leeBoyId + " and internal attachment ID " + attachmentId.ToString() + ".");
            }

            var matches = attachments.Where(a => a.AttachmentID == attachmentId);

            if (matches == null || matches.Count() <= 0)
            {
                throw new ArgumentOutOfRangeException("There are no matching attachments for the claim with LeeBoy ID " + leeBoyId + " and internal attachment ID " + attachmentId.ToString() + ".");
            }

            if (matches.Count() != 1)
            {
                throw new ArgumentOutOfRangeException("More than one matching attachment was found for the claim with LeeBoy ID " + leeBoyId + " and internal attachment ID " + attachmentId.ToString() + ".");
            }

            WarrantyClaimAttachment attachment = matches.Take(1).Single();
            this.LeeBoyID = attachment.LeeBoyID;
            this.AttachmentID = attachment.AttachmentID;
            this.Url = attachment.Url;
            this.Description = attachment.Description;
        }

        public static List<WarrantyClaimAttachment> WarrantyClaimAttachmentsForWarrantyClaim(string leeBoyId)
        {
            List<UD03DataSetTypeUD03> mapped = new List<UD03DataSetTypeUD03>();
            List<WarrantyClaimAttachment> attachments = new List<WarrantyClaimAttachment>();

            try
            {
                AjaxServiceHelper ajax = new AjaxServiceHelper("Ice.BO.UD03Svc", "UD03s", $"Company eq '{Parameters.CompanyID}' and Key1 eq '{leeBoyId}'", typeof(List<UD03>));

                foreach (UD03 item in (List<UD03>)ajax.Value)
                {
                    mapped.Add((UD03DataSetTypeUD03)(new MappingHelper(item, new UD03DataSetTypeUD03()).Value));
                }

                foreach (UD03DataSetTypeUD03 item in mapped)
                {
                    WarrantyClaimAttachment attachment = new WarrantyClaimAttachment();

                    attachment.LeeBoyID = item.Key1;

                    int id = 0;
                    int.TryParse(item.Key5, out id);

                    attachment.AttachmentID = id;

                    attachment.Url = item.Character01;
                    attachment.Description = item.Character02;

                    attachments.Add(attachment);
                }

                return attachments;
            }
            catch (Exception ex)
            {
                return new List<WarrantyClaimAttachment>();
            }
        }
    }
}
