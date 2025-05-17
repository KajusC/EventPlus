import React, { useState, useEffect } from 'react';
import { 
    Container, 
    Typography, 
    Box, 
    Button,
    Paper,
    Grid,
    TextField,
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
    AccordionDetails,
    Chip
} from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { dynamicPricingService } from '../../services/dynamicPricingService';
import { fetchEvents } from '../../services/eventService';
import { fetchSectorPrices } from '../../services/sectorPriceService';
import { useAuth } from '../../context/AuthContext';

function DynamicPricingPanel() {
    const [events, setEvents] = useState([]);
    const [selectedEventId, setSelectedEventId] = useState('');
    const [loading, setLoading] = useState(false);
    const [result, setResult] = useState(null);
    const [error, setError] = useState(null);
    const [snackbarOpen, setSnackbarOpen] = useState(false);
    const [allEvents, setAllEvents] = useState([]);
    const [sectorPrices, setSectorPrices] = useState([]);
    const [previousPrices, setPreviousPrices] = useState({});
    const { currentUser } = useAuth();

    useEffect(() => {
        // Only admin users should access this panel
        if (!currentUser || (currentUser.role !== 'Admin' && currentUser.role !== 'Organiser')) {
            return;
        }

        loadEvents();
        loadSectorPrices();
    }, [currentUser]);

    // Update sector prices when selecting a different event
    useEffect(() => {
        if (selectedEventId) {
            loadSectorPricesForEvent(selectedEventId);
        }
    }, [selectedEventId]);

    const loadEvents = async () => {
        try {
            const fetchedEvents = await fetchEvents();
            
            // Filter for active events (those that haven't ended yet)
            const activeEvents = fetchedEvents.filter(event => {
                return new Date(event.endDate) > new Date();
            });
            
            setAllEvents(fetchedEvents);
            setEvents(activeEvents);
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

    const loadSectorPricesForEvent = async (eventId) => {
        try {
            const prices = await fetchSectorPrices();
            const filteredPrices = prices.filter(price => price.eventId === parseInt(eventId, 10));
            console.log(prices);
            // Save current prices as previous prices for comparison after adjustment
            const priceMap = {};
            filteredPrices.forEach(price => {
                priceMap[`${price.sectorId}-${price.eventId}`] = price.price;
            });
            
            setPreviousPrices(priceMap);
            setSectorPrices(prices);
        } catch (err) {
            console.error('Failed to load sector prices for event:', err);
        }
    };

    const handleEventSelect = (event) => {
        setSelectedEventId(event.target.value);
    };

    const handleAdjustPrice = async () => {
        if (!selectedEventId) {
            setError('Please select an event');
            setSnackbarOpen(true);
            return;
        }

        try {
            setLoading(true);
            setError(null);
            setResult(null);

            const adjustmentResult = await dynamicPricingService.triggerPriceAdjustment(selectedEventId);
            
            // Reload sector prices to show the updated values
            await loadSectorPrices();
            
            setResult(adjustmentResult);
            setSnackbarOpen(true);
        } catch (err) {
            console.error('Error adjusting prices:', err);
            setError(err.message || 'An error occurred');
            setSnackbarOpen(true);
        } finally {
            setLoading(false);
        }
    };

    const handleAdjustAllPrices = async () => {
        try {
            setLoading(true);
            setError(null);
            setResult(null);

            await dynamicPricingService.adjustAllEventPrices();
            
            // Reload sector prices to show the updated values
            await loadSectorPrices();
            
            setResult({ message: 'All event prices have been adjusted' });
            setSnackbarOpen(true);
        } catch (err) {
            console.error('Error adjusting all prices:', err);
            setError(err.message || 'An error occurred');
            setSnackbarOpen(true);
        } finally {
            setLoading(false);
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
        // For now, we'll just return a generic name
        return `Sector ${sectorId}`;
    };

    // Calculate price change percentage
    const getPriceChangePercentage = (currentPrice, key) => {
        const previousPrice = previousPrices[key];
        if (!previousPrice) return 0;
        
        const change = ((currentPrice - previousPrice) / previousPrice) * 100;
        return change;
    };

    // Filter sector prices for the selected event
    const getEventSectorPrices = () => {
        if (!selectedEventId) return [];
        
        return sectorPrices.filter(price => 
            price.eventId === parseInt(selectedEventId, 10)
        );
    };

    // If user is not admin, don't render the component
    if (!currentUser || (currentUser.role !== 'Admin' && currentUser.role !== 'Organiser')) {
        return null;
    }

    return (
        <Container maxWidth="lg" sx={{ mt: 4, mb: 8 }}>
            <Typography variant="h4" component="h1" sx={{ mb: 4, fontWeight: 700 }}>
                Dynamic Pricing Control Panel
            </Typography>

            <Paper elevation={3} sx={{ p: 3, mb: 4 }}>
                <Typography variant="h6" sx={{ mb: 2 }}>
                    Adjust Event Pricing
                </Typography>

                <Grid container spacing={3}>
                    <Grid item xs={12} md={6}>
                        <TextField
                            select
                            fullWidth
                            label="Select Event"
                            value={selectedEventId}
                            onChange={handleEventSelect}
                            SelectProps={{
                                native: true,
                            }}
                            variant="outlined"
                        >
                            <option value="">-- Select Event --</option>
                            {events.map((event) => (
                                <option key={event.idEvent} value={event.idEvent}>
                                    {event.name}
                                </option>
                            ))}
                        </TextField>
                    </Grid>
                    
                    <Grid item xs={12} md={6} sx={{ display: 'flex', alignItems: 'center' }}>
                        <Button 
                            variant="contained" 
                            color="primary"
                            onClick={handleAdjustPrice}
                            disabled={loading || !selectedEventId}
                            sx={{ mr: 2 }}
                        >
                            {loading ? <CircularProgress size={24} /> : 'Adjust Price'}
                        </Button>
                        
                        <Button 
                            variant="outlined" 
                            color="secondary"
                            onClick={handleAdjustAllPrices}
                            disabled={loading}
                        >
                            {loading ? <CircularProgress size={24} /> : 'Adjust All Events'}
                        </Button>
                    </Grid>
                </Grid>

                {result && (
                    <Box sx={{ mt: 3 }}>
                        <Alert severity="success">
                            Price adjustment completed successfully!
                            {result.marketabilityFactor && (
                                <Typography variant="body2" sx={{ mt: 1 }}>
                                    Marketability Factor: {result.marketabilityFactor.toFixed(2)}
                                </Typography>
                            )}
                        </Alert>
                    </Box>
                )}
            </Paper>

            <Paper elevation={3} sx={{ p: 3 }}>
                <Typography variant="h6" sx={{ mb: 2 }}>
                    Event Information
                </Typography>

                <TableContainer>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>Event Name</TableCell>
                                <TableCell>Category</TableCell>
                                <TableCell>Start Date</TableCell>
                                <TableCell>End Date</TableCell>
                                <TableCell>Max Tickets</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {events.map((event) => (
                                <TableRow key={event.idEvent}>
                                    <TableCell>{event.name}</TableCell>
                                    <TableCell>{event.category}</TableCell>
                                    <TableCell>{new Date(event.startDate).toLocaleDateString()}</TableCell>
                                    <TableCell>{new Date(event.endDate).toLocaleDateString()}</TableCell>
                                    <TableCell>{event.maxTicketCount}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
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