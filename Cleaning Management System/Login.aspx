<%@ Page Title="Login - CMS" Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Cleaning Management System | Login</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />

    <style>
        * {
            box-sizing: border-box;
        }

        html, body {
            height: 100%;
            margin: 0;
            font-family: 'Segoe UI', Arial, sans-serif;
        }

        form.login-form {
            height: 100%;
        }

        .login-wrapper {
            position: relative;
            min-height: 100vh;
            width: 100%;
            display: flex;
            align-items: center;
            overflow: hidden;
        }

            
            .login-wrapper::before {
                content: "";
                position: absolute;
                top: 0;
                left: 0;
                right: 0;
                bottom: 0;
                background-image: url('<%= ResolveUrl("~/image/Cleaning.jpg") %>');
                background-size: cover;
                background-position: center;
                background-repeat: no-repeat;
                z-index: 0;
            }

            .login-wrapper::after {
                content: "";
                position: absolute;
                top: 0;
                left: 0;
                right: 0;
                bottom: 0;
                background: linear-gradient(90deg, rgba(10,15,25,0.60) 0%, rgba(10,15,25,0.30) 50%, rgba(10,15,25,0.10) 100%);
                z-index: 1;
            }

        .login-content {
            position: relative;
            z-index: 2;
            width: 100%;
            max-width: 1100px;
            margin: 0 auto;
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 60px 40px;
            gap: 40px;
        }

      
        .welcome-side {
            flex: 1;
            color: #ffffff;
            max-width: 380px;
        }

            .welcome-side h1 {
                font-size: 40px;
                font-weight: 800;
                line-height: 1.15;
                margin: 0 0 16px 0;
                text-shadow: 0 2px 8px rgba(0,0,0,0.4);
            }

            .welcome-side p {
                font-size: 14px;
                line-height: 1.7;
                opacity: 0.92;
                margin: 0 0 22px 0;
                text-shadow: 0 1px 5px rgba(0,0,0,0.4);
            }

        .social-row {
            display: flex;
            gap: 10px;
        }

            .social-row a {
                width: 34px;
                height: 34px;
                border-radius: 6px;
                background: rgba(255,255,255,0.15);
                display: flex;
                align-items: center;
                justify-content: center;
                color: #fff;
                font-size: 14px;
                text-decoration: none;
                transition: background .2s;
            }

                .social-row a:hover {
                    background: rgba(255,255,255,0.3);
                }

       
        .signin-side {
            flex: 1;
            max-width: 340px;
            color: #ffffff;
        }

            .signin-side h2 {
                font-size: 26px;
                font-weight: 800;
                margin: 0 0 22px 0;
                text-shadow: 0 2px 8px rgba(0,0,0,0.4);
            }

        .error-msg {
            display: block;
            color: #fff;
            background: rgba(214, 69, 69, 0.85);
            border-radius: 6px;
            padding: 10px 12px;
            font-size: 13px;
            margin-bottom: 16px;
        }

        .field-group {
            margin-bottom: 16px;
        }

            .field-group label {
                display: block;
                font-size: 13px;
                font-weight: 600;
                margin-bottom: 6px;
                text-shadow: 0 1px 4px rgba(0,0,0,0.4);
            }

            .field-group input.form-control {
                width: 100%;
                border: none;
                border-radius: 4px;
                padding: 11px 14px;
                font-size: 14px;
                outline: none;
                background: #ffffff;
                color: #1a1a1a;
            }

        .remember-row {
            margin-bottom: 18px;
            font-size: 13px;
        }

            .remember-row label {
                display: flex;
                align-items: center;
                gap: 8px;
                cursor: pointer;
                text-shadow: 0 1px 4px rgba(0,0,0,0.4);
            }

        .btn-signin {
            border: none;
            border-radius: 4px;
            background: #E8622D;
            color: #fff;
            font-weight: 700;
            font-size: 14px;
            padding: 12px 26px;
            cursor: pointer;
            transition: background .2s;
        }

            .btn-signin:hover {
                background: #CC5222;
            }

        .lost-password {
            display: block;
            margin-top: 14px;
            font-size: 13px;
            color: #ffffff;
            text-decoration: underline;
        }

            .lost-password:hover {
                opacity: 0.85;
            }

        @media (max-width: 900px) {
            .login-content {
                flex-direction: column;
                align-items: flex-start;
                padding: 40px 24px;
            }

            .welcome-side, .signin-side {
                max-width: 100%;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="login-form" autocomplete="off">
        <div class="login-wrapper">
            <div class="login-content">

                <!-- Left side -->
                <div class="welcome-side">
                    <h1>Cleaning Management System </h1>
                    <p>
                        Manage your cleaning operations, staff, schedules and reports
                        efficiently in one place from the CMS dashboard.
                    </p>

                </div>

                <!-- Right side -->
                <div class="signin-side">
                    <h2>Sign in</h2>

                    <asp:Label ID="lblMessage" runat="server" CssClass="error-msg" Visible="false" />

                    <div class="field-group">
                        <label for="<%= txtUsername.ClientID %>">Username</label>
                        <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control"
                            placeholder="Enter your username" />
                    </div>

                    <div class="field-group">
                        <label for="<%= txtPassword.ClientID %>">Password</label>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control"
                            TextMode="Password" placeholder="Enter your password" />
                    </div>

                    <div class="remember-row">
                        <label>
                            <asp:CheckBox ID="chkRemember" runat="server" />
                            Remember Me
                        </label>
                    </div>

                    <asp:Button ID="btnLogin" runat="server" CssClass="btn-signin" Text="Sign in now"
                        OnClick="btnLogin_Click" />

                    <asp:LinkButton ID="lnkForgot" runat="server" CssClass="lost-password"
                        OnClick="lnkForgot_Click">Forgot your password?</asp:LinkButton>
                </div>

            </div>
        </div>
    </form>
</body>
</html>


