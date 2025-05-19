import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { createEvent } from "../../services/eventService";
import {
	Container,
	Typography,
	Button,
	Box,
	Card,
	CardContent,
	TextField,
	FormControl,
	InputLabel,
	Select,
	MenuItem,
	Tabs,
	Tab,
	IconButton,
	Table,
	TableBody,
	TableCell,
	TableContainer,
	TableHead,
	TableRow,
	Paper,
	Dialog,
	DialogTitle,
	DialogContent,
	DialogActions,
	Tooltip,
	CircularProgress,
    Grid,
    List,
    ListItem,
    ListItemText,
    Alert,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import {
	ArrowBack as ArrowBackIcon,
	Save as SaveIcon,
	Add as AddIcon,
	Delete as DeleteIcon,
	Edit as EditIcon,
	GridView as GridViewIcon,
	Chair as ChairIcon,
} from "@mui/icons-material";
import { fetchCategories } from "../../services/categoryService";

import LoadingSpinner from "../../components/shared/LoadingSpinner";
import ErrorDisplay from "../../components/shared/ErrorDisplay";
import ToastNotification from "../../components/shared/ToastNotification";
import { fetchVenueSuitability } from '../../services/eventService';
import { useAuth } from "../../context/AuthContext";

function EventInsert() {
    const navigate = useNavigate();
    const { currentUser } = useAuth();

	const [sectorDialogOpen, setSectorDialogOpen] = useState(false);
    const [priceDialogOpen, setPriceDialogOpen] = useState(false);
    const [seatingDialogOpen, setSeatingDialogOpen] = useState(false);
    const [bulkDialogOpen, setBulkDialogOpen] = useState(false);

    const [currentSector, setCurrentSector] = useState({name: ''});
    const [currentPrice, setCurrentPrice] = useState({sectorId: '', price: 0});
    const [currentSeating, setCurrentSeating] = useState({sectorId: '', row: '', place: ''});
    const [bulkSeatData, setBulkSeatData] = useState({sectorId: '', rows: 1, seatsPerRow: 1});

    const [editingSector, setEditingSector] = useState(false);
    const [editingPrice, setEditingPrice] = useState(false);
    const [editingSeating, setEditingSeating] = useState(false);
    
    const [venueRecommendations, setVenueRecommendations] = useState([]);
    const [isLoadingRecommendations, setIsLoadingRecommendations] = useState(false);
    const [recommendationsError, setRecommendationsError] = useState(null);
    const [showRecommendations, setShowRecommendations] = useState(false);
    const [selectedRecommendedVenueId, setSelectedRecommendedVenueId] = useState(null);

    const [isLoading, setIsLoading] = useState(true); // Set to true initially for categories
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState(null);
    const [categories, setCategories] = useState([]);
    const [activeTab, setActiveTab] = useState(0);
    const [toast, setToast] = useState({
        open: false,
        message: "",
        severity: "info"
    });

    const [formData, setFormData] = useState({
        event: {
            idEvent: 0,
            name: "",
            description: "",
            startDate: new Date(),
            endDate: new Date(new Date().getTime() + 2 * 60 * 60 * 1000), // Default to 2 hours later
            maxTicketCount: 0, // Default to 0, ensure your validation allows this if intended
            category: '', // Initialize category as empty string
            fkEventLocationidEventLocation: null,
            fkOrganiseridUser: currentUser?.id || 0, // Ensure currentUser is available
            budget : 0,
        },
        eventLocation: {
            name: "",
            address: "",
            city: "",
            country: "",
            capacity: 0,
            contacts: "",
            price: 0,
            idEventLocation: 0,
            idEquipment: 0,
            sectorIds: [], // Initialize as empty array
        },
        partners: {
            name: "",
            description: "",
            website: "",
            idPartner: 0,
        },
        performers: {
            name: "",
            surname: "",
            profession: "",
            idPerformer: 0,
        },
        sectors: [],
        sectorPrices: [],
        seatings: [],
        // eventLocationId: '', // This seems redundant if using eventLocation.idEventLocation
    });

    useEffect(() => {
        const fetchInitialData = async () => {
            setIsLoading(true);
            try {
                const cats = await fetchCategories();
                setCategories(cats);
                if (cats && cats.length > 0) {
                    setFormData((prev) => ({
                        ...prev,
                        event: {
                            ...prev.event,
                            category: cats[0].idCategory, // Set first category as default
                        },
                    }));
                }
                setError(null);
            } catch (err) {
                console.error("Error fetching categories:", err);
                setError("Failed to load categories. Please try again later.");
                setToast({ open: true, message: "Failed to load categories.", severity: "error" });
            } finally {
                setIsLoading(false);
            }
        };
        if (currentUser?.id) { // Ensure organiser ID is set before fetching
            setFormData(prev => ({
                ...prev,
                event: {
                    ...prev.event,
                    fkOrganiseridUser: currentUser.id
                }
            }));
        }
        fetchInitialData();
    }, [currentUser]); // Add currentUser as dependency

    // ... existing handleAddSector, handleEditSector, handleSaveSector ...
    const handleAddSector = () => {
        setCurrentSector({name: ''});
        setEditingSector(false);
        setSectorDialogOpen(true);
    };
    
    const handleEditSector = (sector) => {
        setCurrentSector({...sector});
        setEditingSector(true);
        setSectorDialogOpen(true);
    };
    
    const handleSaveSector = () => {
        if (editingSector) {
            const updatedSectors = formData.sectors.map(s => 
                (s.id === currentSector.id || s.tempId === currentSector.tempId) ? currentSector : s
            );
            setFormData({...formData, sectors: updatedSectors});
        } else {
            const newSector = {
                ...currentSector,
                tempId: `temp-sector-${Date.now()}` // More specific tempId
            };
            setFormData({...formData, sectors: [...formData.sectors, newSector]});
        }
        setSectorDialogOpen(false);
    };
    
    const handleDeleteSector = (sectorIdToDelete) => {
        setFormData(prev => {
            const updatedSectors = prev.sectors.filter(s => 
                (s.id || s.tempId) !== sectorIdToDelete
            );
            const updatedPrices = prev.sectorPrices.filter(p => 
                p.sectorId !== sectorIdToDelete
            );
            const updatedSeatings = prev.seatings.filter(seat => 
                seat.sectorId !== sectorIdToDelete
            );
            return {
                ...prev, 
                sectors: updatedSectors,
                sectorPrices: updatedPrices,
                seatings: updatedSeatings
            };
        });
    };

    const handleAddSectorPrice = () => {
        // Default to the first sector if available, otherwise empty
        const defaultSectorId = formData.sectors.length > 0 ? (formData.sectors[0].id || formData.sectors[0].tempId) : '';
        setCurrentPrice({sectorId: defaultSectorId, price: 0, eventId: formData.event.idEvent || 0 });
        setEditingPrice(false);
        setPriceDialogOpen(true);
    };
    
    const handleEditSectorPrice = (price) => {
        setCurrentPrice({...price, eventId: formData.event.idEvent || 0});
        setEditingPrice(true);
        setPriceDialogOpen(true);
    };
    
    const handleSavePrice = () => {
        const priceToSave = { ...currentPrice, price: parseFloat(currentPrice.price) || 0 };
        if (editingPrice) {
            const updatedPrices = formData.sectorPrices.map(p => 
                ((p.id && p.id === priceToSave.id) || (p.tempId && p.tempId === priceToSave.tempId)) ? priceToSave : p
            );
            setFormData({...formData, sectorPrices: updatedPrices});
        } else {
            const newPriceWithTempId = {
                ...priceToSave,
                tempId: `temp-price-${Date.now()}`
            };
            setFormData({...formData, sectorPrices: [...formData.sectorPrices, newPriceWithTempId]});
        }
        setPriceDialogOpen(false);
    };
    
    const handleDeleteSectorPrice = (priceIdToDelete) => {
        const updatedPrices = formData.sectorPrices.filter(p => 
            (p.id || p.tempId) !== priceIdToDelete
        );
        setFormData({...formData, sectorPrices: updatedPrices});
    };
    
    const handleAddSeating = () => {
        const defaultSectorId = formData.sectors.length > 0 ? (formData.sectors[0].id || formData.sectors[0].tempId) : '';
        setCurrentSeating({
            sectorId: defaultSectorId, 
            row: 1, 
            place: 1
        });
        setEditingSeating(false);
        setSeatingDialogOpen(true);
    };
    
    const handleEditSeating = (seat) => {
        setCurrentSeating({...seat});
        setEditingSeating(true);
        setSeatingDialogOpen(true);
    };

    const handleSaveSeating = () => {
        const seatToSave = { 
            ...currentSeating, 
            row: parseInt(currentSeating.row, 10) || 1,
            place: parseInt(currentSeating.place, 10) || 1
        };
        if (editingSeating) {
            const updatedSeatings = formData.seatings.map(s => 
                ((s.id && s.id === seatToSave.id) || (s.tempId && s.tempId === seatToSave.tempId)) ? seatToSave : s
            );
            setFormData({...formData, seatings: updatedSeatings});
        } else {
            const newSeatingWithTempId = {
                ...seatToSave,
                tempId: `temp-seat-${Date.now()}`
            };
            setFormData({...formData, seatings: [...formData.seatings, newSeatingWithTempId]});
        }
        setSeatingDialogOpen(false);
    };
    
    const handleDeleteSeating = (seatIdToDelete) => {
        const updatedSeatings = formData.seatings.filter(s => 
            (s.id || s.tempId) !== seatIdToDelete
        );
        setFormData({...formData, seatings: updatedSeatings});
    };
    
    const handleBulkAddSeats = () => {
        const defaultSectorId = formData.sectors.length > 0 ? (formData.sectors[0].id || formData.sectors[0].tempId) : '';
        setBulkSeatData({
            sectorId: defaultSectorId,
            rows: 5,
            seatsPerRow: 10
        });
        setBulkDialogOpen(true);
    };
    
    const handleBulkSeatsSave = () => {
        const { sectorId, rows, seatsPerRow } = bulkSeatData;
        if (!sectorId) {
            setToast({ open: true, message: "Please select a sector for bulk adding seats.", severity: "warning" });
            return;
        }
        const newSeats = [];
        const numRows = parseInt(rows, 10) || 0;
        const numSeatsPerRow = parseInt(seatsPerRow, 10) || 0;

        for (let r = 1; r <= numRows; r++) {
            for (let p = 1; p <= numSeatsPerRow; p++) {
                newSeats.push({
                    sectorId, // This will be the actual ID or tempId of the sector
                    row: r,
                    place: p,
                    tempId: `temp-seat-${Date.now()}-${r}-${p}`
                });
            }
        }
        
        setFormData(prev => ({
            ...prev,
            seatings: [...prev.seatings, ...newSeats]
        }));
        
        setBulkDialogOpen(false);
    };

    const handleInputChange = (section, field, value) => {
        setFormData((prev) => ({
            ...prev,
            [section]: {
                ...prev[section],
                [field]: value,
            },
        }));
    };

    const handleDateChange = (section, field) => (newDate) => {
        setFormData((prev) => ({
            ...prev,
            [section]: {
                ...prev[section],
                [field]: newDate instanceof Date && !isNaN(newDate) ? newDate : null,
            },
        }));
    };

    const handleTabChange = (event, newValue) => {
        setActiveTab(newValue);
    };

    const handleGetVenueRecommendations = async () => {
        setIsLoadingRecommendations(true);
        setRecommendationsError(null);
        setVenueRecommendations([]);
        setShowRecommendations(true);

        const { name, category, maxTicketCount, startDate, endDate } = formData.event;
        
        // Client-side validation before API call
        if (!name) {
            setToast({ open: true, message: "Event Name is required to get recommendations.", severity: "warning" });
            setIsLoadingRecommendations(false);
            return;
        }
        if (!category) { // Check if category is empty or null
            setToast({ open: true, message: "Category is required to get recommendations.", severity: "warning" });
            setIsLoadingRecommendations(false);
            return;
        }
        if (maxTicketCount === undefined || maxTicketCount === null || parseInt(maxTicketCount, 10) < 0) {
            setToast({ open: true, message: "Valid Max Ticket Count (0 or more) is required.", severity: "warning" });
            setIsLoadingRecommendations(false);
            return;
        }
        if (!startDate || !endDate) {
            setToast({ open: true, message: "Start and End Dates are required.", severity: "warning" });
            setIsLoadingRecommendations(false);
            return;
        }
        if (endDate < startDate) {
            setToast({ open: true, message: "End Date must be on or after Start Date.", severity: "warning" });
            setIsLoadingRecommendations(false);
            return;
        }

        const eventCriteria = {
            name,
            category: parseInt(category, 10),
            maxTicketCount: parseInt(maxTicketCount, 10),
            startDate: startDate.toISOString().split("T")[0],
            endDate: endDate.toISOString().split("T")[0],
            description: formData.event.description || "",
            budget: parseFloat(formData.event.budget) || 0, // Naudojame biudžetą rekomendacijoms
        };

        try {
            console.log("Requesting venue suitability with criteria:", eventCriteria);
            const result = await fetchVenueSuitability(eventCriteria);
            console.log("Venue suitability response:", result);
            setVenueRecommendations(result.recommendedVenues || []);
            if (result.recommendedVenues && result.recommendedVenues.length > 0) {
                setToast({ open: true, message: result.message || "Venue recommendations loaded!", severity: "success" });
                setActiveTab(1); 
            } else {
                setToast({ open: true, message: result.message || "No suitable venues found based on criteria.", severity: "info" });
            }
        } catch (err) {
            console.error("Error fetching venue recommendations:", err);
            const errorMessage = err?.errors ? Object.values(err.errors).flat().join(' ') : (err.message || 'Failed to fetch venue recommendations.');
            setRecommendationsError(errorMessage);
            setToast({ open: true, message: `Error: ${errorMessage}`, severity: "error" });
        } finally {
            setIsLoadingRecommendations(false);
        }
    };

    const handleSelectRecommendedVenue = (venue) => {
        setFormData(prev => ({
            ...prev,
            eventLocation: { 
                name: venue.name || "",
                address: venue.address || "",
                city: venue.city || "",
                country: venue.country || "",
                capacity: venue.capacity !== undefined ? parseInt(venue.capacity, 10) : 0,
                contacts: venue.contacts || "",
                price: venue.price !== undefined ? parseFloat(venue.price) : 0,
                idEventLocation: 0, // Svarbu: nustatome 0, kad būtų sukurta NAUJA lokacija
                idEquipment: venue.idEquipment !== undefined ? parseInt(venue.idEquipment, 10) : 0,
                sectorIds: [],
            },
            event: { 
                ...prev.event,
                fkEventLocationidEventLocation: null, // Nustatome null, kad serveryje būtų sukurtas naujas įrašas
            }
        }));
        // Vietoj ID naudokime venue name kaip indikatorių
    setSelectedRecommendedVenueId(venue.name); 
    setShowRecommendations(false); 
    setToast({ 
        open: true, 
        message: `Vieta "${venue.name}" pasirinkta. Bus sukurta nauja vieta šiam renginiui.`, 
        severity: "info" 
    });
    };

    const handleClearSelectedVenue = () => {
        setFormData(prev => ({
            ...prev,
            eventLocation: { 
                name: "", address: "", city: "", country: "",
                capacity: 0, contacts: "", price: 0,
                idEventLocation: 0, 
                idEquipment: 0, sectorIds: [],
            },
            event: {
                ...prev.event,
                fkEventLocationidEventLocation: null, 
            }
        }));
        setSelectedRecommendedVenueId(null);
        setShowRecommendations(true); 
    };

    const handleSubmit = async () => {
        setIsSubmitting(true);
        setError(null);

        // Final validation before submission
        if (formData.event.endDate < formData.event.startDate) {
            setToast({ open: true, message: "End Date must be on or after Start Date.", severity: "error" });
            setIsSubmitting(false);
            return;
        }
        if (!formData.event.name || !formData.event.category) {
            setToast({ open: true, message: "Event Name and Category are required.", severity: "error" });
            setIsSubmitting(false);
            return;
        }
        
        if (formData.sectors.length > 0 && 
        (!formData.eventLocation.name || 
         (formData.eventLocation.idEventLocation <= 0 && 
          !formData.eventLocation.address && !formData.eventLocation.city))) {
        setToast({ 
            open: true, 
            message: "You must specify a location when adding sectors.", 
            severity: "error" 
        });
            setIsSubmitting(false);
            return;
        }

        const dataToSubmit = {
            event: {
                ...formData.event,
                startDate: formData.event.startDate.toISOString().split("T")[0],
                endDate: formData.event.endDate.toISOString().split("T")[0],
                maxTicketCount: parseInt(formData.event.maxTicketCount, 10) || 0,
                category: parseInt(formData.event.category, 10),
                fkOrganiseridUser: currentUser?.id || formData.event.fkOrganiseridUser,
                budget: parseFloat(formData.event.budget) || 0,
            },
            eventLocation: formData.eventLocation.name || formData.eventLocation.idEventLocation > 0 ? {
                ...formData.eventLocation,
                capacity: parseInt(formData.eventLocation.capacity, 10) || 0,
                price: parseFloat(formData.eventLocation.price) || 0,
                idEquipment: parseInt(formData.eventLocation.idEquipment, 10) || 0,
                sectorIds: Array.isArray(formData.eventLocation.sectorIds) ? 
                    formData.eventLocation.sectorIds.map(id => parseInt(id, 10)).filter(id => !isNaN(id)) : [],
            } : null,
            partners: formData.partners.name ? formData.partners : { name: "", description: "", website: "", idPartner: 0 },
            performers: formData.performers.name || formData.performers.surname ? formData.performers : { name: "", surname: "", profession: "", idPerformer: 0 },
            sectors: formData.sectors.map(sector => {
                const locationId = formData.eventLocation.idEventLocation > 0 ? 
                    formData.eventLocation.idEventLocation : 0;
                    
                return {
                    name: sector.name,
                    id: sector.id || 0,
                    tempId: sector.tempId,
                    fkEventLocationidEventLocation: locationId,
                };
            }),
            sectorPrices: formData.sectorPrices.map(price => ({
                price: parseFloat(price.price) || 0,
                sectorId: price.sectorId,
                id: price.id || 0,
                eventId: formData.event.idEvent || 0,
                tempId: price.tempId,
            })),
            seatings: formData.seatings.map(seat => ({
                row: parseInt(seat.row, 10) || 0,
                place: parseInt(seat.place, 10) || 0,
                sectorId: seat.sectorId,
                id: seat.id || 0,
                tempId: seat.tempId,
            }))
        };
      
        console.log("Submitting data to createEvent:", JSON.stringify(dataToSubmit, null, 2));
        try {
            await createEvent(dataToSubmit);
            setToast({
                open: true,
                message: "Event created successfully!",
                severity: "success"
            });
            
            // Nukreipimas į renginių sąrašą po sėkmingo sukūrimo
            setTimeout(() => {
                navigate('/events');
            }, 2000);
        } catch (err) {
            console.error("Create event error:", err);
            setError(err.message || "Failed to create event");
            setToast({
                open: true, 
                message: err.message || "Failed to create event", 
                severity: "error"
            });
        } finally {
            setIsSubmitting(false);
        }
    };

    const handleCloseToast = () => {
        setToast({ ...toast, open: false });
    };

    if (isLoading && categories.length === 0) return <LoadingSpinner message="Loading event creation form..." />; // Show spinner while categories load
    // Removed the early return for error here to allow the form to render and show toast for category loading failure

    return (
        <Container maxWidth="lg" sx={{ mt: 4, mb: 8 }}> {/* Adjusted mt for better spacing with nav */}
            <Box sx={{ display: "flex", alignItems: "center", mb: 3, pt: { xs: 2, md: 0 } }}> {/* Adjusted pt */}
                <IconButton
                    onClick={() => navigate("/events")}
                    sx={{ mr: 2, color: 'primary.main' }}
                    aria-label="go back"
                >
                    <ArrowBackIcon />
                </IconButton>
                <Typography variant="h4" component="h1" fontWeight={700}>
                    Create New Event
                </Typography>
            </Box>

            {error && !isLoading && categories.length === 0 && ( // Show error for category loading if it failed
                <ErrorDisplay error={error} marginTop={2} marginBottom={2} />
            )}

            <Card sx={{ mb: 4, borderRadius: 3, boxShadow: '0 8px 30px -5px rgba(0,0,0,0.1)' }}>
                <CardContent sx={{ p: { xs: 2, md: 4 } }}>
                    <Tabs
                        value={activeTab}
                        onChange={handleTabChange}
                        variant="scrollable"
                        scrollButtons="auto"
                        allowScrollButtonsMobile
                        sx={{ 
                            mb: 3, 
                            borderBottom: 1, 
                            borderColor: "divider",
                            '& .MuiTab-root': { fontWeight: 600, textTransform: 'none', fontSize: '1rem' }
                        }}
                    >
                        <Tab label="Event Details" />
                        <Tab label="Location" />
                        <Tab label="Partner (Optional)" />
                        <Tab label="Performer (Optional)" />
                        <Tab label="Sectors & Seating" />
                    </Tabs>

                    <form onSubmit={(e) => e.preventDefault()}>
                        {/* Event Details Tab */}
                        {activeTab === 0 && (
                            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2.5 }}>
                                <TextField
                                    fullWidth
                                    label="Event Name"
                                    name="name" // for easier targeting if needed
                                    value={formData.event.name}
                                    onChange={(e) =>
                                        handleInputChange("event", "name", e.target.value)
                                    }
                                    required
                                    variant="outlined"
                                />
                                <TextField
                                    fullWidth
                                    label="Description"
                                    name="description"
                                    value={formData.event.description}
                                    onChange={(e) =>
                                        handleInputChange("event", "description", e.target.value)
                                    }
                                    multiline
                                    rows={4}
                                    variant="outlined"
                                />
                                <Grid container spacing={2.5}>
                                    <Grid item xs={12} sm={6}>
                                        <FormControl fullWidth variant="outlined" required>
                                            <InputLabel>Category</InputLabel>
                                            <Select
                                                name="category"
                                                value={formData.event.category}
                                                onChange={(e) =>
                                                    handleInputChange("event", "category", e.target.value)
                                                }
                                                label="Category"
                                            >
                                                <MenuItem value="">
                                                    <em>Select Category</em>
                                                </MenuItem>
                                                {categories.map((category) => (
                                                    <MenuItem
                                                        key={category.idCategory}
                                                        value={category.idCategory}
                                                    >
                                                        {category.name}
                                                    </MenuItem>
                                                ))}
                                            </Select>
                                        </FormControl>
                                    </Grid>
                                    <Grid item xs={12} sm={6}>
                                        <TextField
                                            fullWidth
                                            type="number"
                                            label="Max Ticket Count"
                                            name="maxTicketCount"
                                            value={formData.event.maxTicketCount}
                                            onChange={(e) =>
                                                handleInputChange(
                                                    "event",
                                                    "maxTicketCount",
                                                    e.target.value
                                                )
                                            }
                                            required
                                            InputProps={{ inputProps: { min: 0 } }}
                                            variant="outlined"
                                        />
                                    </Grid>
                                    <Grid item xs={12} sm={4}>
                                        <TextField
                                            fullWidth
                                            type="number"
                                            label="Organizer Budget (for recommendations)"
                                            name="budget"
                                            value={formData.event.budget}
                                            onChange={(e) =>
                                                handleInputChange("event", "budget", e.target.value)
                                            }
                                            InputProps={{ inputProps: { min: 0, step: "1" } }}
                                            variant="outlined"
                                            helperText="Only used for venue recommendations"
                                        />
                                    </Grid>
                                </Grid>
                                <Grid container spacing={2.5}>
                                    <Grid item xs={12} sm={6}>
                                        <LocalizationProvider dateAdapter={AdapterDateFns}>
                                            <DatePicker
                                                label="Start Date"
                                                value={formData.event.startDate}
                                                onChange={handleDateChange("event", "startDate")}
                                                slotProps={{
                                                    textField: { fullWidth: true, required: true, variant: "outlined" },
                                                }}
                                            />
                                        </LocalizationProvider>
                                    </Grid>
                                    <Grid item xs={12} sm={6}>
                                        <LocalizationProvider dateAdapter={AdapterDateFns}>
                                            <DatePicker
                                                label="End Date"
                                                value={formData.event.endDate}
                                                onChange={handleDateChange("event", "endDate")}
                                                minDate={formData.event.startDate} // Min end date is start date
                                                slotProps={{
                                                    textField: { fullWidth: true, required: true, variant: "outlined" },
                                                }}
                                            />
                                        </LocalizationProvider>
                                    </Grid>
                                </Grid>
                                <Box sx={{ mt: 2, textAlign: 'center' }}>
                                    <Button
                                        variant="contained"
                                        color="secondary"
                                        onClick={handleGetVenueRecommendations}
                                        disabled={isLoadingRecommendations || !formData.event.name || !formData.event.category || formData.event.maxTicketCount < 0 || !formData.event.startDate || !formData.event.endDate}
                                        startIcon={isLoadingRecommendations ? <CircularProgress size={20} color="inherit" /> : <GridViewIcon />}
                                        sx={{ py: 1.5, px: 3, fontWeight: 600 }}
                                    >
                                        {isLoadingRecommendations ? "Loading Venues..." : "Find Suitable Venues"}
                                    </Button>
                                </Box>
                            </Box>
                        )}

                        {/* Location Tab */}
                        {activeTab === 1 && (
                            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2.5 }}>
                                {isLoadingRecommendations && <LoadingSpinner message="Fetching recommendations..." />}
                                {recommendationsError && <ErrorDisplay error={recommendationsError} marginTop={2} marginBottom={2}/>}

                                {!isLoadingRecommendations && !recommendationsError && showRecommendations && venueRecommendations.length > 0 && !selectedRecommendedVenueId && (
                                    <Paper elevation={1} sx={{ p: 2, border: '1px solid', borderColor: 'divider' }}>
                                        <Typography variant="h6" gutterBottom>Recommended Venues</Typography>
                                        <List>
                                            {venueRecommendations.map(venue => (
                                                <ListItem 
                                                    key={venue.idEventLocation}
                                                    secondaryAction={
                                                        <Button variant="outlined" size="small" onClick={() => handleSelectRecommendedVenue(venue)}>
                                                            Select Venue
                                                        </Button>
                                                    }
                                                    divider
                                                    sx={{ '&:last-child': { borderBottom: 0 } }}
                                                >
                                                    <ListItemText
                                                        primary={venue.name}
                                                        secondary={`Capacity: ${venue.capacity} | City: ${venue.city} | Price: ${venue.price !== undefined ? venue.price.toFixed(2) : 'N/A'}`}
                                                    />
                                                </ListItem>
                                            ))}
                                        </List>
                                        <Button sx={{mt: 1.5, textTransform: 'none'}} onClick={() => { setShowRecommendations(false); setSelectedRecommendedVenueId(null); }}>
                                            Or Enter Location Manually
                                        </Button>
                                    </Paper>
                                )}

                                {(!showRecommendations || selectedRecommendedVenueId || (venueRecommendations.length === 0 && !isLoadingRecommendations && !recommendationsError)) && (
                                    <>
                                        {selectedRecommendedVenueId && (
                                            <Alert severity="info" sx={{ mb: 2 }} action={
                                                <Button color="inherit" size="small" onClick={handleClearSelectedVenue}>
                                                    CHANGE
                                                </Button>
                                            }>
                                                Selected venue: <strong>{formData.eventLocation.name}</strong>. You can proceed or change selection.
                                            </Alert>
                                        )}
                                        <TextField
                                            fullWidth
                                            label="Location Name"
                                            value={formData.eventLocation.name}
                                            onChange={(e) => handleInputChange("eventLocation", "name", e.target.value)}
                                            required={!selectedRecommendedVenueId} // Required if not selecting a recommendation
                                            disabled={!!selectedRecommendedVenueId}
                                            variant="outlined"
                                        />
                                        <TextField
                                            fullWidth
                                            label="Address"
                                            value={formData.eventLocation.address}
                                            onChange={(e) => handleInputChange("eventLocation", "address", e.target.value)}
                                            required={!selectedRecommendedVenueId}
                                            disabled={!!selectedRecommendedVenueId}
                                            variant="outlined"
                                        />
                                        <Grid container spacing={2.5}>
                                            <Grid item xs={12} sm={6}>
                                                <TextField
                                                    fullWidth
                                                    label="City"
                                                    value={formData.eventLocation.city}
                                                    onChange={(e) => handleInputChange("eventLocation", "city", e.target.value)}
                                                    required={!selectedRecommendedVenueId}
                                                    disabled={!!selectedRecommendedVenueId}
                                                    variant="outlined"
                                                />
                                            </Grid>
                                            <Grid item xs={12} sm={6}>
                                                <TextField
                                                    fullWidth
                                                    label="Country"
                                                    value={formData.eventLocation.country}
                                                    onChange={(e) => handleInputChange("eventLocation", "country", e.target.value)}
                                                    required={!selectedRecommendedVenueId}
                                                    disabled={!!selectedRecommendedVenueId}
                                                    variant="outlined"
                                                />
                                            </Grid>
                                        </Grid>
                                        <Grid container spacing={2.5}>
                                            <Grid item xs={12} sm={4}>
                                                <TextField
                                                    fullWidth
                                                    type="number"
                                                    label="Capacity"
                                                    value={formData.eventLocation.capacity}
                                                    onChange={(e) => handleInputChange("eventLocation", "capacity", e.target.value)}
                                                    required={!selectedRecommendedVenueId}
                                                    disabled={!!selectedRecommendedVenueId}
                                                    InputProps={{ inputProps: { min: 0 } }}
                                                    variant="outlined"
                                                />
                                            </Grid>
                                            <Grid item xs={12} sm={4}>
                                                <TextField
                                                    fullWidth
                                                    type="number"
                                                    label="Price (Venue Rent)"
                                                    value={formData.eventLocation.price}
                                                    onChange={(e) => handleInputChange("eventLocation", "price", e.target.value)}
                                                    // required={!selectedRecommendedVenueId} // Price might be optional
                                                    disabled={!!selectedRecommendedVenueId}
                                                    InputProps={{ inputProps: { min: 0, step: "0.01" } }}
                                                    variant="outlined"
                                                />
                                            </Grid>
                                            <Grid item xs={12} sm={4}>
                                                <TextField
                                                    fullWidth
                                                    type="number"
                                                    label="Equipment ID (Optional)"
                                                    value={formData.eventLocation.idEquipment}
                                                    onChange={(e) => handleInputChange("eventLocation", "idEquipment", e.target.value)}
                                                    disabled={!!selectedRecommendedVenueId}
                                                    InputProps={{ inputProps: { min: 0 } }}
                                                    variant="outlined"
                                                />
                                            </Grid>
                                        </Grid>
                                        <TextField
                                            fullWidth
                                            label="Contacts (Optional)"
                                            value={formData.eventLocation.contacts}
                                            onChange={(e) => handleInputChange("eventLocation", "contacts", e.target.value)}
                                            disabled={!!selectedRecommendedVenueId}
                                            variant="outlined"
                                        />
                                    </>
                                )}
                            </Box>
                        )}

                        {/* Partners Tab */}
                        {activeTab === 2 && (
                            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2.5 }}>
                                <TextField
                                    fullWidth
                                    label="Partner Name"
                                    value={formData.partners.name}
                                    onChange={(e) =>
                                        handleInputChange("partners", "name", e.target.value)
                                    }
                                    variant="outlined"
                                />
                                <TextField
                                    fullWidth
                                    label="Description"
                                    value={formData.partners.description}
                                    onChange={(e) =>
                                        handleInputChange(
                                            "partners",
                                            "description",
                                            e.target.value
                                        )
                                    }
                                    multiline
                                    rows={3}
                                    variant="outlined"
                                />
                                <TextField
                                    fullWidth
                                    label="Website"
                                    value={formData.partners.website}
                                    onChange={(e) =>
                                        handleInputChange("partners", "website", e.target.value)
                                    }
                                    variant="outlined"
                                />
                            </Box>
                        )}

                        {/* Performers Tab */}
                        {activeTab === 3 && (
                            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2.5 }}>
                                <Grid container spacing={2.5}>
                                    <Grid item xs={12} sm={6}>
                                        <TextField
                                            fullWidth
                                            label="First Name"
                                            value={formData.performers.name}
                                            onChange={(e) =>
                                                handleInputChange("performers", "name", e.target.value)
                                            }
                                            variant="outlined"
                                        />
                                    </Grid>
                                    <Grid item xs={12} sm={6}>
                                        <TextField
                                            fullWidth
                                            label="Last Name"
                                            value={formData.performers.surname}
                                            onChange={(e) =>
                                                handleInputChange("performers", "surname", e.target.value)
                                            }
                                            variant="outlined"
                                        />
                                    </Grid>
                                </Grid>
                                <TextField
                                    fullWidth
                                    label="Profession"
                                    value={formData.performers.profession}
                                    onChange={(e) =>
                                        handleInputChange(
                                            "performers",
                                            "profession",
                                            e.target.value
                                        )
                                    }
                                    variant="outlined"
                                />
                            </Box>
                        )}

                        {/* Seating Tab (Sectors, Prices, Seats) */}
                        {activeTab === 4 && (
                            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
                                {selectedRecommendedVenueId && (
                                    <Alert severity="info" sx={{ mb: 1 }}>
                                        You are creating sectors and seats specific to this event. These will not affect other events at this venue.
                                    </Alert>
                                )}
                                {/* Sectors Management Section */}
                                <Paper elevation={1} sx={{ p: 2, border: '1px solid', borderColor: 'divider' }}>
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1.5 }}>
                                        <Typography variant="h6">Sectors</Typography>
                                        <Button 
                                            variant="contained" 
                                            size="small" 
                                            startIcon={<AddIcon />}
                                            onClick={handleAddSector}
                                        >
                                            Add Sector
                                        </Button>
                                    </Box>
                                    <TableContainer>
                                        <Table size="small">
                                            <TableHead>
                                                <TableRow>
                                                    <TableCell sx={{fontWeight: 'bold'}}>Name</TableCell>
                                                    <TableCell align="right" sx={{fontWeight: 'bold'}}>Actions</TableCell>
                                                </TableRow>
                                            </TableHead>
                                            <TableBody>
                                                {formData.sectors.length > 0 ? (
                                                    formData.sectors.map((sector) => (
                                                        <TableRow key={sector.id || sector.tempId} hover>
                                                            <TableCell>{sector.name}</TableCell>
                                                            <TableCell align="right">
                                                                <Tooltip title="Edit Sector">
                                                                    <IconButton size="small" onClick={() => handleEditSector(sector)}>
                                                                        <EditIcon fontSize="inherit" />
                                                                    </IconButton>
                                                                </Tooltip>
                                                                <Tooltip title="Delete Sector">
                                                                    <IconButton 
                                                                        size="small" 
                                                                        color="error"
                                                                        onClick={() => handleDeleteSector(sector.id || sector.tempId)}
                                                                    >
                                                                        <DeleteIcon fontSize="inherit" />
                                                                    </IconButton>
                                                                </Tooltip>
                                                            </TableCell>
                                                        </TableRow>
                                                    ))
                                                ) : (
                                                    <TableRow>
                                                        <TableCell colSpan={2} align="center" sx={{ fontStyle: 'italic', color: 'text.secondary' }}>No sectors added yet.</TableCell>
                                                    </TableRow>
                                                )}
                                            </TableBody>
                                        </Table>
                                    </TableContainer>
                                </Paper>
                                
                                {/* Sector Prices Section */}
                                <Paper elevation={1} sx={{ p: 2, border: '1px solid', borderColor: 'divider' }}>
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1.5 }}>
                                        <Typography variant="h6">Sector Prices</Typography>
                                        <Button 
                                            variant="contained" 
                                            size="small" 
                                            startIcon={<AddIcon />}
                                            onClick={handleAddSectorPrice}
                                            disabled={formData.sectors.length === 0}
                                        >
                                            Add Price
                                        </Button>
                                    </Box>
                                    <TableContainer>
                                        <Table size="small">
                                            <TableHead>
                                                <TableRow>
                                                    <TableCell sx={{fontWeight: 'bold'}}>Sector</TableCell>
                                                    <TableCell sx={{fontWeight: 'bold'}}>Price</TableCell>
                                                    <TableCell align="right" sx={{fontWeight: 'bold'}}>Actions</TableCell>
                                                </TableRow>
                                            </TableHead>
                                            <TableBody>
                                                {formData.sectorPrices.length > 0 ? (
                                                    formData.sectorPrices.map((price) => (
                                                        <TableRow key={price.id || price.tempId} hover>
                                                            <TableCell>
                                                                {formData.sectors.find(s => (s.id || s.tempId) === price.sectorId)?.name || 'N/A'}
                                                            </TableCell>
                                                            <TableCell>${(parseFloat(price.price) || 0).toFixed(2)}</TableCell>
                                                            <TableCell align="right">
                                                                <Tooltip title="Edit Price">
                                                                    <IconButton size="small" onClick={() => handleEditSectorPrice(price)}>
                                                                        <EditIcon fontSize="inherit" />
                                                                    </IconButton>
                                                                </Tooltip>
                                                                <Tooltip title="Delete Price">
                                                                    <IconButton 
                                                                        size="small" 
                                                                        color="error"
                                                                        onClick={() => handleDeleteSectorPrice(price.id || price.tempId)}
                                                                    >
                                                                        <DeleteIcon fontSize="inherit" />
                                                                    </IconButton>
                                                                </Tooltip>
                                                            </TableCell>
                                                        </TableRow>
                                                    ))
                                                ) : (
                                                    <TableRow>
                                                        <TableCell colSpan={3} align="center" sx={{ fontStyle: 'italic', color: 'text.secondary' }}>No prices added yet.</TableCell>
                                                    </TableRow>
                                                )}
                                            </TableBody>
                                        </Table>
                                    </TableContainer>
                                </Paper>
                                
                                {/* Seats Management */}
                                <Paper elevation={1} sx={{ p: 2, border: '1px solid', borderColor: 'divider' }}>
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1.5 }}>
                                        <Typography variant="h6">Seats</Typography>
                                        <Box>
                                            <Button 
                                                variant="contained" 
                                                size="small" 
                                                startIcon={<AddIcon />}
                                                onClick={handleAddSeating}
                                                disabled={formData.sectors.length === 0}
                                                sx={{ mr: 1 }}
                                            >
                                                Add Seat
                                            </Button>
                                            <Button
                                                variant="outlined"
                                                size="small"
                                                startIcon={<ChairIcon />}
                                                onClick={handleBulkAddSeats}
                                                disabled={formData.sectors.length === 0}
                                            >
                                                Bulk Add
                                            </Button>
                                        </Box>
                                    </Box>
                                    <TableContainer>
                                        <Table size="small">
                                            <TableHead>
                                                <TableRow>
                                                    <TableCell sx={{fontWeight: 'bold'}}>Sector</TableCell>
                                                    <TableCell sx={{fontWeight: 'bold'}}>Row</TableCell>
                                                    <TableCell sx={{fontWeight: 'bold'}}>Place</TableCell>
                                                    <TableCell align="right" sx={{fontWeight: 'bold'}}>Actions</TableCell>
                                                </TableRow>
                                            </TableHead>
                                            <TableBody>
                                                {formData.seatings.length > 0 ? (
                                                    formData.seatings.map((seat) => (
                                                        <TableRow key={seat.id || seat.tempId} hover>
                                                            <TableCell>
                                                                {formData.sectors.find(s => (s.id || s.tempId) === seat.sectorId)?.name || 'N/A'}
                                                            </TableCell>
                                                            <TableCell>{seat.row}</TableCell>
                                                            <TableCell>{seat.place}</TableCell>
                                                            <TableCell align="right">
                                                                <Tooltip title="Edit Seat">
                                                                    <IconButton size="small" onClick={() => handleEditSeating(seat)}>
                                                                        <EditIcon fontSize="inherit" />
                                                                    </IconButton>
                                                                </Tooltip>
                                                                <Tooltip title="Delete Seat">
                                                                    <IconButton 
                                                                        size="small" 
                                                                        color="error"
                                                                        onClick={() => handleDeleteSeating(seat.id || seat.tempId)}
                                                                    >
                                                                        <DeleteIcon fontSize="inherit" />
                                                                    </IconButton>
                                                                </Tooltip>
                                                            </TableCell>
                                                        </TableRow>
                                                    ))
                                                ) : (
                                                    <TableRow>
                                                        <TableCell colSpan={4} align="center" sx={{ fontStyle: 'italic', color: 'text.secondary' }}>No seats added yet.</TableCell>
                                                    </TableRow>
                                                )}
                                            </TableBody>
                                        </Table>
                                    </TableContainer>
                                </Paper>
                            </Box>
                        )}

                        <Box
                            sx={{ display: "flex", justifyContent: "space-between", mt: 4, pt: 2, borderTop: 1, borderColor: 'divider' }}
                        >
                            <Button
                                variant="outlined"
                                onClick={() => setActiveTab(activeTab > 0 ? activeTab - 1 : 0)}
                                disabled={activeTab === 0}
                            >
                                Previous
                            </Button>
                            
                            {activeTab < 4 ? (
                                <Button
                                    variant="contained"
                                    onClick={() => setActiveTab(activeTab + 1)}
                                >
                                    Next
                                </Button>
                            ) : (
                                <Button
                                    type="button" // pakeitimas į "button" 
                                    variant="contained"
                                    color="primary"
                                    onClick={handleSubmit} // tiesioginis iškvietimas
                                    disabled={isSubmitting}
                                    startIcon={isSubmitting ? <CircularProgress size={20} color="inherit"/> : <SaveIcon />}
                                    sx={{ py: 1.5, px: 4, fontWeight: 600 }}
                                >
                                    {isSubmitting ? "Creating Event..." : "Create Event"}
                                </Button>
                            )}
                        </Box>
                    </form>
                </CardContent>
            </Card>

            {/* Sector Dialog */}
            <Dialog open={sectorDialogOpen} onClose={() => setSectorDialogOpen(false)} fullWidth maxWidth="xs">
                <DialogTitle>{editingSector ? 'Edit Sector' : 'Add New Sector'}</DialogTitle>
                <DialogContent>
                    <TextField
                        autoFocus
                        margin="dense"
                        label="Sector Name"
                        fullWidth
                        variant="outlined"
                        value={currentSector.name}
                        onChange={(e) => setCurrentSector({...currentSector, name: e.target.value})}
                        required
                    />
                </DialogContent>
                <DialogActions sx={{ px:3, pb: 2 }}>
                    <Button onClick={() => setSectorDialogOpen(false)}>Cancel</Button>
                    <Button onClick={handleSaveSector} variant="contained" disabled={!currentSector.name}>Save</Button>
                </DialogActions>
            </Dialog>

            {/* Sector Price Dialog */}
            <Dialog open={priceDialogOpen} onClose={() => setPriceDialogOpen(false)} fullWidth maxWidth="xs">
                <DialogTitle>{editingPrice ? 'Edit Sector Price' : 'Add New Sector Price'}</DialogTitle>
                <DialogContent>
                    <FormControl fullWidth margin="dense" variant="outlined" required>
                        <InputLabel>Sector</InputLabel>
                        <Select
                            value={currentPrice.sectorId}
                            onChange={(e) => setCurrentPrice({...currentPrice, sectorId: e.target.value})}
                            label="Sector"
                            disabled={formData.sectors.length === 0}
                        >
                            {formData.sectors.length === 0 && <MenuItem value="" disabled><em>No sectors available</em></MenuItem>}
                            {formData.sectors.map((sector) => (
                                <MenuItem key={sector.id || sector.tempId} value={sector.id || sector.tempId}>
                                    {sector.name}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                    <TextField
                        margin="dense"
                        label="Price"
                        type="number"
                        fullWidth
                        variant="outlined"
                        value={currentPrice.price}
                        onChange={(e) => setCurrentPrice({...currentPrice, price: e.target.value})}
                        InputProps={{ inputProps: { min: 0, step: "0.01" } }}
                        required
                    />
                </DialogContent>
                <DialogActions sx={{ px:3, pb: 2 }}>
                    <Button onClick={() => setPriceDialogOpen(false)}>Cancel</Button>
                    <Button onClick={handleSavePrice} variant="contained" disabled={!currentPrice.sectorId || currentPrice.price < 0}>Save</Button>
                </DialogActions>
            </Dialog>

            {/* Seating Dialog */}
            <Dialog open={seatingDialogOpen} onClose={() => setSeatingDialogOpen(false)} fullWidth maxWidth="xs">
                <DialogTitle>{editingSeating ? 'Edit Seat' : 'Add New Seat'}</DialogTitle>
                <DialogContent>
                    <FormControl fullWidth margin="dense" variant="outlined" required>
                        <InputLabel>Sector</InputLabel>
                        <Select
                            value={currentSeating.sectorId}
                            onChange={(e) => setCurrentSeating({...currentSeating, sectorId: e.target.value})}
                            label="Sector"
                            disabled={formData.sectors.length === 0}
                        >
                            {formData.sectors.length === 0 && <MenuItem value="" disabled><em>No sectors available</em></MenuItem>}
                            {formData.sectors.map((sector) => (
                                <MenuItem key={sector.id || sector.tempId} value={sector.id || sector.tempId}>
                                    {sector.name}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                    <TextField
                        margin="dense"
                        label="Row Number"
                        type="number"
                        fullWidth
                        variant="outlined"
                        value={currentSeating.row}
                        onChange={(e) => setCurrentSeating({...currentSeating, row: e.target.value})}
                        InputProps={{ inputProps: { min: 1 } }}
                        required
                    />
                    <TextField
                        margin="dense"
                        label="Place Number"
                        type="number"
                        fullWidth
                        variant="outlined"
                        value={currentSeating.place}
                        onChange={(e) => setCurrentSeating({...currentSeating, place: e.target.value})}
                        InputProps={{ inputProps: { min: 1 } }}
                        required
                    />
                </DialogContent>
                <DialogActions sx={{ px:3, pb: 2 }}>
                    <Button onClick={() => setSeatingDialogOpen(false)}>Cancel</Button>
                    <Button onClick={handleSaveSeating} variant="contained" disabled={!currentSeating.sectorId || !currentSeating.row || !currentSeating.place}>Save</Button>
                </DialogActions>
            </Dialog>

            {/* Bulk Add Seats Dialog */}
            <Dialog open={bulkDialogOpen} onClose={() => setBulkDialogOpen(false)} fullWidth maxWidth="xs">
                <DialogTitle>Bulk Add Seats</DialogTitle>
                <DialogContent>
                    <FormControl fullWidth margin="dense" variant="outlined" required>
                        <InputLabel>Target Sector</InputLabel>
                        <Select
                            value={bulkSeatData.sectorId}
                            onChange={(e) => setBulkSeatData({...bulkSeatData, sectorId: e.target.value})}
                            label="Target Sector"
                            disabled={formData.sectors.length === 0}
                        >
                            {formData.sectors.length === 0 && <MenuItem value="" disabled><em>No sectors available</em></MenuItem>}
                            {formData.sectors.map((sector) => (
                                <MenuItem key={sector.id || sector.tempId} value={sector.id || sector.tempId}>
                                    {sector.name}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                    <TextField
                        margin="dense"
                        label="Number of Rows"
                        type="number"
                        fullWidth
                        variant="outlined"
                        value={bulkSeatData.rows}
                        onChange={(e) => setBulkSeatData({...bulkSeatData, rows: e.target.value})}
                        InputProps={{ inputProps: { min: 1 } }}
                        required
                    />
                    <TextField
                        margin="dense"
                        label="Seats per Row"
                        type="number"
                        fullWidth
                        variant="outlined"
                        value={bulkSeatData.seatsPerRow}
                        onChange={(e) => setBulkSeatData({...bulkSeatData, seatsPerRow: e.target.value})}
                        InputProps={{ inputProps: { min: 1 } }}
                        required
                    />
                </DialogContent>
                <DialogActions sx={{ px:3, pb: 2 }}>
                    <Button onClick={() => setBulkDialogOpen(false)}>Cancel</Button>
                    <Button onClick={handleBulkSeatsSave} variant="contained" disabled={!bulkSeatData.sectorId || bulkSeatData.rows < 1 || bulkSeatData.seatsPerRow < 1}>Create Seats</Button>
                </DialogActions>
            </Dialog>

            <ToastNotification
                open={toast.open}
                message={toast.message}
                severity={toast.severity}
                onClose={handleCloseToast}
            />
        </Container>
    );
}

export default EventInsert;