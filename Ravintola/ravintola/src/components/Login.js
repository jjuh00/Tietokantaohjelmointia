import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router';
import { authentication  } from '../utils/authentication';
import '../styles/Login.css';

const Login = () => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();
        try {
            const result = await authentication.login(email, password);
            
            if (result.success) {
                navigate("/main");
            } else {
                setError(result.error);
            }
        } catch (error) {
            setError("Virhe kirjautumisessa. Yritä uudelleen.");
            console.error("Kirjautumisvirhe: ", error)
        }
    };

    return (
        <div className="login-container">
            <h2>Kirjautuminen</h2>
            <form onSubmit={handleLogin}>
                {error && <div className="error-message">{error}</div>}

                <div className="form-item">
                    <label htmlFor="email">Sähköposti:</label>
                    <input 
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>

                <div className="form-item">
                    <label htmlFor="password">Salasana:</label>
                    <input 
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>

                <button type="submit" className="login-btn">Kirjaudu</button>

                <div className="register-link"><p>Eikö sinulla tiliä? <Link to="/register">Rekisteröidy tästä</Link></p></div>
            </form>
        </div>
    );
}

export default Login;