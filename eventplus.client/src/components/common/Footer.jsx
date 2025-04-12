import React from 'react';

function Footer() {
    return (
        <footer style={{ width: '100vw', textAlign: 'center', padding: '20px', backgroundColor: '#2B2D42', margin: '0' }}>
            <p>&copy; {new Date().getFullYear()} EventPlus. All rights reserved.</p>
        </footer>
    );
}

export default Footer;