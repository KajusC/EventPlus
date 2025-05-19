import React, { useState, useEffect } from 'react';
import { 
    Container, 
    Typography, 
    Box, 
    Button,
    Paper,
    Grid,
    CircularProgress,
    Alert,
    Snackbar,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Accordion,
    AccordionSummary,
    AccordionDetails
} from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import AutorenewIcon from '@mui/icons-material/Autorenew';
import { fetchEvents } from '../../services/eventService';
import { fetchSectorPrices } from '../../services/sectorPriceService';
import { useAuth } from '../../context/AuthContext';
import { adjustAllEventPrices } from '../../services/ticketService';

function DynamicPricingPanel() {
    const [loading, setLoading] = useState(false);
    const [globalLoading, setGlobalLoading] = useState(false);
    const [result, setResult] = useState(null);
    const [error, setError] = useState(null);
    const [snackbarOpen, setSnackbarOpen] = useState(false);
    const [allEvents, setAllEvents] = useState([]);
    const [sectorPrices, setSectorPrices] = useState([]);
    const [nextUpdateTime, setNextUpdateTime] = useState(null);
    const { currentUser } = useAuth();

    useEffect(() => {
        // Only admin users should access this panel
        if (!currentUser || (currentUser.role !== 'Administrator' && currentUser.role !== 'Organiser')) {
            return;
        }

        loadEvents();
        loadSectorPrices();
        
        
        // Set up polling to update the next update time
        const interval = setInterval(() => {
            updateNextUpdateTimeDisplay();
        }, 60000); // Update every minute
        
        // Initial display
        updateNextUpdateTimeDisplay();
        
        return () => clearInterval(interval);
    }, [currentUser]);
    
    const updateNextUpdateTimeDisplay = () => {
        const lastScheduledTime = localStorage.getItem('lastPriceUpdateSchedule');
        if (lastScheduledTime) {
            const nextUpdate = parseInt(lastScheduledTime, 10) + 24 * 60 * 60 * 1000;
            setNextUpdateTime(new Date(nextUpdate));
        }
    };

    const loadEvents = async () => {
        try {
            const fetchedEvents = await fetchEvents();
            setAllEvents(fetchedEvents);
        } catch (err) {
            console.error('Failed to load events:', err);
            setError('Failed to load events');
        }
    };

    const loadSectorPrices = async () => {
        try {
            const prices = await fetchSectorPrices();
            setSectorPrices(prices);
        } catch (err) {
            console.error('Failed to load sector prices:', err);
            setError('Failed to load sector prices');
        }
    };
    
    // Handle the global price update for all events
    const handleGlobalPriceUpdate = async () => {
        try {
            setGlobalLoading(true);
            setError(null);
            setResult(null);

            await adjustAllEventPrices();
            
            await loadSectorPrices();
            
            setResult({ message: 'Global price update completed successfully' });
            setSnackbarOpen(true);
            
            localStorage.removeItem('lastPriceUpdateSchedule');
            
            updateNextUpdateTimeDisplay();
        } catch (err) {
            console.error('Error performing global price update:', err);
            setError(err.message || 'An error occurred during global price update');
            setSnackbarOpen(true);
        } finally {
            setGlobalLoading(false);
        }
    };

    const handleCloseSnackbar = () => {
        setSnackbarOpen(false);
    };

    // Get event name by ID
    const getEventName = (eventId) => {
        const event = allEvents.find(e => e.idEvent === eventId);
        return event ? event.name : 'Unknown Event';
    };

    // Get sector name by ID
    const getSectorName = (sectorId) => {
        // This is a placeholder - in a real app, you'd fetch sector names from your API
        return `Sector ${sectorId}`;
    };

    // Format time until next update
    const formatTimeUntilNextUpdate = () => {
        if (!nextUpdateTime) return 'Not scheduled';
        
        const now = new Date();
        const diffMs = nextUpdateTime - now;
        
        if (diffMs <= 0) return 'Update due now';
        
        const diffHrs = Math.floor(diffMs / (1000 * 60 * 60));
        const diffMins = Math.floor((diffMs % (1000 * 60 * 60)) / (1000 * 60));
        
        return `${diffHrs}h ${diffMins}m`;
    };

    // If user is not admin, don't render the component
    if (!currentUser || (currentUser.role !== 'Administrator' && currentUser.role !== 'Organiser')) {
        return null;
    }

    return (
        <Container maxWidth="lg" sx={{ mt: 4, mb: 8 }}>
            <Typography variant="h4" component="h1" sx={{ mb: 4, fontWeight: 700 }}>
                Dynamic Pricing Control Panel
            </Typography>

            {/* Global Price Update Section */}
            <Paper elevation={3} sx={{ p: 3, mb: 4, borderLeft: '4px solid #2196f3' }}>
                <Typography variant="h6" sx={{ mb: 2, display: 'flex', alignItems: 'center' }}>
                    <AutorenewIcon sx={{ mr: 1 }} /> Global Price Update System
                </Typography>
                
                <Grid container spacing={3} alignItems="center">
                    <Grid item xs={12} md={6}>
                        <Box>
                            <Typography variant="body1" sx={{ mb: 1 }}>
                                This system automatically updates all event prices every 24 hours.
                            </Typography>
                            <Typography variant="body2" color="text.secondary">
                                Next scheduled update: {nextUpdateTime ? nextUpdateTime.toLocaleString() : 'Not scheduled'}
                            </Typography>
                            <Typography variant="body2" color="text.secondary">
                                Time until next update: {formatTimeUntilNextUpdate()}
                            </Typography>
                        </Box>
                    </Grid>
                    
                    <Grid item xs={12} md={6} sx={{ display: 'flex', justifyContent: 'flex-end' }}>
                        <Button 
                            variant="contained" 
                            color="primary"
                            startIcon={<AutorenewIcon />}
                            onClick={handleGlobalPriceUpdate}
                            disabled={globalLoading}
                        >
                            {globalLoading ? 
                                <CircularProgress size={24} color="inherit" /> : 
                                'Run Update Now'
                            }
                        </Button>
                    </Grid>
                </Grid>
            </Paper>

            <Accordion sx={{ mt: 4 }}>
                <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                    <Typography variant="h6">All Sector Prices</Typography>
                </AccordionSummary>
                <AccordionDetails>
                    <TableContainer>
                        <Table>
                            <TableHead>
                                <TableRow>
                                    <TableCell>Event</TableCell>
                                    <TableCell>Sector</TableCell>
                                    <TableCell>Price</TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {sectorPrices.map((price) => (
                                    <TableRow key={`${price.sectorId}-${price.eventId}`}>
                                        <TableCell>{getEventName(price.eventId)}</TableCell>
                                        <TableCell>{getSectorName(price.sectorId)}</TableCell>
                                        <TableCell>${parseFloat(price.price).toFixed(2)}</TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </TableContainer>
                </AccordionDetails>
            </Accordion>

            <Snackbar 
                open={snackbarOpen} 
                autoHideDuration={6000} 
                onClose={handleCloseSnackbar}
                anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
            >
                <Alert 
                    onClose={handleCloseSnackbar} 
                    severity={error ? "error" : "success"} 
                    variant="filled"
                >
                    {error || (result?.message || "Price adjustment completed successfully!")}
                </Alert>
            </Snackbar>
        </Container>
    );
}

export default DynamicPricingPanel;