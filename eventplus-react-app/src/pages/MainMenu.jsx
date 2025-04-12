import React from 'react';
import { Link } from 'react-router-dom';

function MainMenu() {
    return (
        <div>
            <h1>Main Menu</h1>
            <nav>
                <ul>
                    <li>
                        <Link to="/events">View Events</Link>
                    </li>
                    <li>
                        <Link to="/eventinsert">Insert Event</Link>
                    </li>
                    <li>
                        <Link to="/eventedit">Edit Event</Link>
                    </li>
                    <li>
                        <Link to="/eventview">Event Details</Link>
                    </li>
                </ul>
            </nav>
        </div>
    );
}

export default MainMenu;