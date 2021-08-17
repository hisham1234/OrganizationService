﻿using System;
namespace Organization_Service.Models
{

    /**
     * This class is matching login's answer including the token information.
     */
    public class LoginAnswerModel
    {
        public string Token { get; set; }

        public string Message { get; set; }
    }
}
