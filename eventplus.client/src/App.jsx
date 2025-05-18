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
import UserRequestAnswersPage from './pages/userRequestAnswer/UserRequestAnswerPage';
import QuestionManagement from './pages/userRequestAnswer/QuestionManagement';
import TicketListPage from './pages/tickets/TicketListPage';
import TicketView from './pages/tickets/TicketView';
import TicketEditPage from './pages/tickets/TicketEditPage';
import TicketPurchasePage from './pages/tickets/TicketPurchacePage';
import DynamicPricingPanel from './pages/admin/DynamicPricingPanel';
import { initializeDynamicPricing } from './services/setUpDynamicPricing';
import TicketScanPage from './pages/tickets/TicketScanPage';

function App() {
    initializeDynamicPricing(false);
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
                        <Route path="/tickets" element={<TicketListPage />} />
                        <Route path="/ticket/:id" element={<TicketView />} />
                        <Route path="/eventview/:id" element={<EventView />} />
                        <Route path="/login" element={<Login />} />
                        <Route path="/register" element={<Register />} />
                        <Route path="tickets/TicketPurchasePage/:eventId" element={<TicketPurchasePage />} />
                        
                        
                        {/* Protected routes (any authenticated user) */}
                        <Route element={<ProtectedRoute />}>
                            <Route path="/profile" element={<Profile />} />
                            <Route path="/requests" element={<UserRequestAnswersPage />} />
                        </Route>
                        
                        {/* Organizer routes */}
                        <Route element={<ProtectedRoute requireOrganizer={true} />}>
                            <Route path="/eventinsert" element={<EventInsert />} />
                            <Route path="/eventedit/:id" element={<EventEdit />} />
                            {/* TO DO/ change to admin \/ */}
                            <Route path="/ticket/edit/:id" element={<TicketEditPage />} />
                            <Route path="/organiser/scan-ticket" element={<TicketScanPage />} />
                            {/* Add more organizer-only routes here */}
                        </Route>
                        
                        {/* Admin routes */}
                        <Route element={<ProtectedRoute requireAdmin={true} />}>
                            {/* Add admin-only routes here */}
                            <Route path="/admin" element={<QuestionManagement />} />
                            <Route path="/admin/dynamic-pricing" element={<DynamicPricingPanel />} />
                        </Route>
                    </Routes>
                    <Footer />
                </Router>
            </NotificationProvider>
        </AuthProvider>
    );
}

export default App;