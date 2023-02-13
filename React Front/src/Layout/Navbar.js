import {React} from "react"


export default function Navbar(props) {
    
    const userDisplayName = "test"
    const userCoins = 10
    const isLoggedIn = true

    return (
        <nav className="navbar navbar-expand-lg navbar-dark bg-primary">

            <div className="container-fluid">

                <p>home</p>
        
                <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarColor01" aria-controls="navbarColor01" aria-expanded="false" aria-label="Toggle navigation">
                    <span className="navbar-toggler-icon"></span>
                </button>
        
                <div className="collapse navbar-collapse" id="navbarColor01">
            
                    <ul className="navbar-nav">
                
                        <li className="nav-item">
                            <p>home</p>
                        </li>

                        <li className="nav-item me-auto">
                            <p>Users</p>
                        </li>

                    </ul>

                    {isLoggedIn ? 
                        <ul className="navbar-nav ms-auto">
                            <li className="nav-item navbar-text text-success me-5">
                                <h5><strong><span id="coin-balance">{userCoins}</span></strong></h5>
                            </li>

                            <li className="nav-item text-dark">
                                <p>{userDisplayName}</p>
                            </li>

                            <li className="nav-item">
                                <p>Generate</p>
                            </li>

                            <li className="nav-item">

                                <p>Log out</p>

                                <form method="post" asp-page="/Identity/Logout">
                                    <button hidden type="submit" id="logout-button" className="nav-link" > Log out </button>
                                </form>

                            </li>

                        </ul> 
                    : 
                        <ul className="navbar-nav ms-auto">

                            <li className="nav-item">
                                <p>Log in</p>
                            </li>
                    
                            <li className="nav-item">
                                <p>Register</p>
                            </li>
                    
                        </ul>
                    }

                </div>

            </div>

        </nav>
    )
}