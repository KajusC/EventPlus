import React from 'react';
import { Link } from 'react-router-dom';
import Header from '../components/common/Header';
import { Card } from '@mui/material';

const styles = {
  container: {
    width: "100%",
    height: "100vh",
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    position: 'relative',
  },
  image: {
    width: "100%",
    height: "100%",
    objectFit: "cover",
  },
  link: {
    backgroundColor: "#8D99AE",
    color: "white",
    padding: "12px 24px",
    borderRadius: "8px",
    textDecoration: "none",
    marginRight: "15px",
    display: "inline-block",
    fontSize: "16px",
  },
  card: {
    position: "absolute",
    top: "50%",
    right: "5%",
    transform: "translateY(-50%)",
    padding: "40px",
    backgroundColor: "rgba(255, 255, 255, 0.9)",
    borderRadius: "15px",
    boxShadow: "0 8px 16px rgba(0, 0, 0, 0.2)",
    maxWidth: "400px",
    textAlign: "center",
  }
};

function MainMenu() {
    return (
      <>
        <div style={{ width: "100%", height: "100%", position: 'relative' }}>
          <img
            src="public\pexels-teddy-2263436.jpg"
            alt="Main Menu"
            style={{
              width: "100%",
              height: "100%",
              objectFit: "cover",
            }}
          />
          <Card
            style={styles.card}
          >
            <h1 style={{ color: "#333", marginBottom: "20px" }}>Welcome to EventPlus</h1>
            <p style={{ color: "#666", fontSize: "16px" }}>
              Your one-stop solution for finding most popular events!  Tickets are stock-based.
            </p>
            <div style={{ marginTop: "30px" }}>
              <Link
                to="/events"
                style={styles.link}
              >
                View Events
              </Link>
              <Link
                to="/organiser/scan-ticket"
                style={styles.link}
              >
                Scan Ticket (Organiser)
              </Link>
            </div>
          </Card>
        </div>
      </>
    );
}

export default MainMenu;