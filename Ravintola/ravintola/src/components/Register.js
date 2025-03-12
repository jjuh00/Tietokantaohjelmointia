import React, { useState } from 'react';
import { useNavigate } from 'react-router';
import { authentication } from '../utils/authentication';
import '../styles/Register.css';

const Register = () => {
    const [name, setName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [balance, setBalance] = useState("");
    const [error, setError] = useState("");
    const navigate = useNavigate();

    const handleRegister = async (e) => {
        e.preventDefault();

        try {
            if (!name || !email || !password || !balance) {
                setError("Täytä kaikki kentät")
                return;
            }

            // Tarkistetaan, onko annettu rahamäärä oikeanlainen
            const balanceNum = parseFloat(balance);
            if (isNaN(balanceNum) || balanceNum < 0) {
                setError("Virheellinen rahamäärä");
                return;
            }

            const result = await authentication.register(email, password, balanceNum);

            if (result.success) {
                navigate("/main");
            } else {
                setError(result.error)
            }
        } catch (error) {
            setError("Tapahtui virhe rekisteröityessä. Yritä uudelleen.");
            console.error("Rekisteröitivirhe: ", error);
        }
    };

    return (
        <div className="register-container">
            <h2>Rekisteröityminen</h2>
            <form onSubmit={handleRegister}>
                {error && <div className="error-message">{error}</div>}

                <div className="form-item">
                    <label htmlFor="name">Nimi:</label>
                    <input 
                        type="text"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                        required
                    />
                </div>

                <div className="form-item">
                    <label htmlFor="email">Sähköposti</label>
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

                <div className="form-item">
                    <label htmlFor="balance">Rahat (€):</label>
                    <input 
                        type="number"
                        value={balance}
                        onChange={(e) => setBalance(e.target.value)}
                        step="0.01"
                        min="0"
                        required
                    />
                </div>

                <button type="submit" className="register-btn">Rekisteröidy</button>
            </form>
        </div>
    );
}

export default Register;