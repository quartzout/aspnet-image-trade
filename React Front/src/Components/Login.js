import {React, useState} from "react"
import { useNavigate } from "react-router-dom"
import useAuth from "../Auth/useAuth"

export default function Login(props) {
    
    const navigate = useNavigate()
    
    const { fetchUser } = useAuth() 

    const [formData, setFormData] = useState({
        email: "",
        password: "",
        rememberMe: false
    })

    const emptyValidationData = {
        global: [],
        displayName: [],
        email: [],
        password: [],
        confirmPassword: []
    }

    const [validationData, setValidationData] = useState(emptyValidationData)

    function onFormChanged(event) {
        setFormData(prevFormData => ({
            ...prevFormData,
            [event.target.name]: event.target.type === "checkbox" ? event.target.checked : event.target.value
        }))
    }

    function submit() {
        (async () => {
            const response = await fetch(process.env.REACT_APP_API_HOST +  "/api/account/login/", {
                method: "POST",
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify(formData)
            })
            if (response.ok) {
                fetchUser()
                navigate("/")
            }
            else {
                const content = await response.json();
                let newValidationData = structuredClone(emptyValidationData)
                for (const key in content.errors) {

                    const jsKey = key === "" ? "global" : key[0].toLowerCase() + key.slice(1) 
                    newValidationData[jsKey] = content.errors[key]
                }
                setValidationData(newValidationData)
            }


        })()
        
    }


    return <>

        <form>
    
            <p>Link to register here</p>
    
            <div className="container">

                <label for="email" ><b>Email</b></label>
                <input type="text" placeholder="Enter email" id="email" name="email" value={formData.email} onChange={onFormChanged}/> <br/>
                {validationData.email !== [] && <ul>{validationData.email.map(val => <li className="text-danger">{val}</li>)}</ul> }

                <br/><br/>
    
                <label for="password"><b>Password</b></label>
                <input type="password" placeholder="Enter Password" id="password" name="password" value={formData.password} onChange={onFormChanged}/><br/>
                {validationData.password !== [] && <ul>{validationData.password.map(val => <li className="text-danger">{val}</li>)}</ul> }
                
                <br/><br/>
    
                <label for="rememberMe"><b>Remember Me</b></label>
                <input type="checkbox" id="rememberMe" name="rememberMe" checked={formData.rememberMe} onChange={onFormChanged}/> 
                <br/> <br/>
    
                {validationData.global !== [] && <ul>{validationData.global.map(val => <li className="text-danger">{val}</li>)}</ul> }
    
                <button type="button" onClick={submit}>Login</button>
    
            </div>
    
        </form>
    
    </>
}