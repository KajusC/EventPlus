import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Header from './components/common/Header';
import Footer from './components/common/Footer';
import Navigation from './components/common/Navigation';
import MainMenu from './pages/MainMenu';
import EventView from './pages/events/EventView';
import EventList from './pages/events/EventList';
import EventInsert from './pages/events/EventInsert';
import EventEdit from './pages/events/EventEdit';
import Login from './pages/auth/Login';
import Register from './pages/auth/Register';
import Profile from './pages/auth/Profile';
import ProtectedRoute from './components/common/ProtectedRoute';
import { AuthProvider } from './context/AuthContext';
import NotificationProvider from './context/NotificationContext';

function App() {
    return (
        <AuthProvider>
            <NotificationProvider>
                <Router>
                    {/* <Header /> */}
                    <Navigation />
                    <Routes>
                        {/* Public routes */}
                        <Route path="/" element={<MainMenu />} />
                        <Route path="/events" element={<EventList />} />
                        <Route path="/eventview/:id" element={<EventView />} />
                        <Route path="/login" element={<Login />} />
                        <Route path="/register" element={<Register />} />
                        
                        {/* Protected routes (any authenticated user) */}
                        <Route element={<ProtectedRoute />}>
                            <Route path="/profile" element={<Profile />} />
                        </Route>
                        
                        {/* Organizer routes */}
                        <Route element={<ProtectedRoute requireOrganizer={true} />}>
                            <Route path="/eventinsert" element={<EventInsert />} />
                            <Route path="/eventedit/:id" element={<EventEdit />} />
                            {/* Add more organizer-only routes here */}
                        </Route>
                        
                        {/* Admin routes */}
                        <Route element={<ProtectedRoute requireAdmin={true} />}>
                            {/* Add admin-only routes here */}
                        </Route>
                    </Routes>
                    <Footer />
                </Router>
            </NotificationProvider>
        </AuthProvider>
    );
}

export default App;