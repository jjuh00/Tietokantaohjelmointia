import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router';
import Login from './components/Login';
import Register from './components/Register';
import Main from './components/Main';
import './styles/App.css';

const App = () => {
    return (
        <Router>
            <div className="app">
                <Routes>
                    <Route path="/" element={<Login />} />
                    <Route path="/register" element={<Register />} />
                    <Route path="/main" element={<Main />} />
                    <Route path="*" element={<Navigate to="/" />} />
                </Routes>
            </div>
        </Router>
    );
}

export default App;