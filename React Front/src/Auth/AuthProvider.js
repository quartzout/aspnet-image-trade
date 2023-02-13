import { React, useState } from "react";
import { UserContext } from "./UserContext";


export default function AuthProvider({ children }) {

    const [user, setUser] = useState(null);

    function fetchUser() {
        (async () => {
            const response = await fetch(process.env.REACT_APP_API_HOST + "/api/account/getcurrentuser/", {
                method: "GET",
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                credentials: "include"
            })
            console.log(response)
            if (response.ok) {
                setUser(await response.json())
            }
            
        })()
        
    }

    function removeUser() {
        setUser(null)
    } 

    return <>
        <UserContext.Provider value={{user, fetchUser, removeUser}}>
            {children}
        </UserContext.Provider>
    </>
}