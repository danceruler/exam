using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FP_wab.Models
{
 
    public enum AnnounceType
    {
        SUCCESS = 1,
        FAIL = 2,
    }
    public enum QuestionType
    {
        TYPE_RADIO,
        TYPE_MULTIPLE,
        TYPE_TRUE_FALSE,
        TYPE_BLANK,
        TYPE_ANSWER,
        TYPE_OPERATION
    }
}