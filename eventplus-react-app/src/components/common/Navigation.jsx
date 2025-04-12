import { Link } from 'react-router-dom';

function Navigation() {
    return (
        <nav>
            <ul>
                <li>
                    <Link to="/">Home</Link>
                </li>
                <li>
                    <Link to="/events">Events</Link>
                </li>
                <li>
                    <Link to="/eventinsert">Insert Event</Link>
                </li>
                <li>
                    <Link to="/eventlist">Event List</Link>
                </li>
                <li>
                    <Link to="/mainmenu">Main Menu</Link>
                </li>
            </ul>
        </nav>
    );
}

export default Navigation;