import {React, useState} from "react"
import { useNavigate } from "react-router-dom"
import useAuth from "../Auth/useAuth";


export default function Login(props) {

    const navigate = useNavigate();

    const { fetchUser } = useAuth()



    const [formData, setFormData] = useState({
        email: "",
        displayName: "", 
        confirmPassword: "",
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
        const {confirmPassword, ...objectToSend} = formData
        if (confirmPassword !== formData.password) {
            setValidationData({
                ...emptyValidationData,
                confirmPassword: ["Passwords do not match"]
            })
            return
        }
        (async () => {
            const response = await fetch(process.env.REACT_APP_API_HOST + "/api/account/register/", {
                method: "POST",
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                credentials: "include",
                body: JSON.stringify(objectToSend)
            })
            if (response.ok) {
                fetchUser()
                navigate("/")
            }
            else {
                const content = await response.json(); 
                let newValidationData = structuredClone(emptyValidationData) //если не клонировать, в цикле будут меняться свойства прошлой validationData
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
    
            <p>Link to login here</p>
    
            <div className="container">

                <label for="email" ><b>Email</b></label>
                <input type="text" placeholder="Enter email" id="email" name="email" value={formData.email} onChange={onFormChanged}/> <br/>
                {validationData.email !== [] && <ul>{validationData.email.map(val => <li className="text-danger">{val}</li>)}</ul> }

                <br/><br/>

                <label for="displayName" ><b>Display name</b></label>
                <input type="text" placeholder="Enter name" id="displayName" name="displayName" value={formData.displayName} onChange={onFormChanged}/> <br/>
                {validationData.displayName !== [] && <ul>{validationData.displayName.map(val => <li className="text-danger">{val}</li>)}</ul> }

                <br/><br/>
    
                <label for="password"><b>Password</b></label>
                <input type="password" placeholder="Enter Password" id="password" name="password" value={formData.password} onChange={onFormChanged}/><br/>
                {validationData.password !== [] && <ul>{validationData.password.map(val => <li className="text-danger">{val}</li>)}</ul> }
                
                <br/><br/>

                <label for="confirmPassword"><b>confirmPassword</b></label>
                <input type="confirmPassword" placeholder="Confirm password" id="confirmPassword" name="confirmPassword" value={formData.confirmPassword} onChange={onFormChanged}/><br/>
                {validationData.confirmPassword !== [] && <ul>{validationData.confirmPassword.map(val => <li className="text-danger">{val}</li>)}</ul> }
                
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