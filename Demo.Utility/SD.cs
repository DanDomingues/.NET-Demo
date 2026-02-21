using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Utility
{
    public static class SD
    {
        public const string ROLE_USER_CUSTOMER = "Customer";
        public const string ROLE_USER_COMPANY = "Company";
        public const string ROLE_USER_ADMIN = "Admin";
        public const string ROLE_USER_EMPLOYEE = "Employee";

        public const string ORDER_STATUS_PENDING = "Pending";
        public const string ORDER_STATUS_APPROVED = "Approved";
        public const string ORDER_STATUS_PROCESSING = "Processing";
        public const string ORDER_STATUS_SHIPPED = "Shipped";
        public const string ORDER_STATUS_CANCELLED = "Cancelled";

        public const string PAYMENT_STATUS_PENDING = "Pending";
        public const string PAYMENT_STATUS_APPROVED = "Approved";
        public const string PAYMENT_STATUS_DELAYED = "ApprovedForDelayedPayment";
        public const string PAYMENT_STATUS_REFUNDED = "Refunded";
        public const string PAYMENT_STATUS_REJECTED = "Rejected";
    }
}
