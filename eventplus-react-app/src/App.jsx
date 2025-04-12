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
import Navbar from './components/common/Navbar';

function App() {
    return (
        <Router>
            <Navbar />
            <Header />
            <Navigation />
            <Routes>
                <Route path="/" element={<MainMenu />} />
                <Route path="/events" element={<EventList />} />
                <Route path="/eventview/:id" element={<EventView />} />
                <Route path="/eventinsert" element={<EventInsert />} />
                <Route path="/eventedit/:id" element={<EventEdit />} />
            </Routes>
            <Footer />
        </Router>
    );
}

export default App;