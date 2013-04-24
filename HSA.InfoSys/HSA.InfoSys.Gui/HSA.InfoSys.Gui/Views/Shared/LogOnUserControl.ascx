<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated) {
%>
        <ul class="nav nav-font" style="margin-right: 0px; margin-left: 309px;">
            <li class="dropdown">
                <a href="" class="dropdown-toggle" data-toggle="dropdown" style="padding: 7px 3px 6px;">
                    <span class="label label-info" style="padding: 6px 8px;">
                        <i class="icon-wrench icon-white"></i>
                    </span>
                </a>
                <ul class="dropdown-menu">
                    <li><a href="/Account/Changepassword">Change Password</a></li>
                </ul>
            </li>
            <li id="user">
                <span class="label label-info pull-right" style="padding: 6px 8px; margin: 7px 3px 6px; height: 15px;">
                <%= Page.User.Identity.Name %>
                </span>
            </li>
            <li id="logout"><a href="/Account/LogOff">Logout</a></li>
            
        </ul>
<%
    }
    else {
%> 
        <ul class="nav nav-font pull-right">
            <li id="login"><a href="/Account/LogOn">Login</a></li>
            <li id="register"><a href="/Account/Register">Register</a></li>
        </ul>

       <!-- <form class="navbar-form pull-right" action="/Account/LogOn">
            <input class="span2 textbox-small" type="text" placeholder="Email">
            <input class="span2 textbox-small" type="password" placeholder="Password">
            <button type="submit" class="btn btn-small">Sign in</button>
            <a href="/Account/Register" class="btn btn-small">Register</a>
        </form> -->
<%
    }
%>