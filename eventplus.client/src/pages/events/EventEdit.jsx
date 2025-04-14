import React, { useState, useEffect } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import { fetchEventById, updateEvent } from "../../services/eventService";
import {
  Container,
  Typography,
  Button,
  Box,
  CircularProgress,
  Alert,
  Grid,
  Card,
  CardContent,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Stack,
  Snackbar,
  Alert as MuiAlert,
  Paper,
  IconButton,
  Divider,
  Chip
} from "@mui/material";
import { DateTimePicker } from "@mui/x-date-pickers/DateTimePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { 
  Edit as EditIcon, 
  Save as SaveIcon, 
  ArrowBack as ArrowBackIcon,
  Event as EventIcon,
  Description as DescriptionIcon,
  Category as CategoryIcon,
  ConfirmationNumber as TicketIcon,
  CalendarMonth as CalendarIcon
} from "@mui/icons-material";

// Category name mapping
const getCategoryName = (categoryId) => {
  const categories = {
    1: 'Music',
    2: 'Business',
    // Add more mappings as needed
    default: 'Event'
  };
  return categories[categoryId] || categories.default;
};

// Event category to image mapping
const getCategoryImage = (category) => {
  const images = {
    1: 'https://source.unsplash.com/random/1200x400/?concert',
    2: 'https://source.unsplash.com/random/1200x400/?conference',
    // Add more mappings as needed
    default: 'https://source.unsplash.com/random/1200x400/?event'
  };
  return images[category] || images.default;
};

function EventEdit() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [submitLoading, setSubmitLoading] = useState(false);
  const [formData, setFormData] = useState({
    name: "",
    description: "",
    category: 0,
    startDate: new Date(),
    endDate: new Date(),
    maxTicketCount: 0,
  });
  const [toast, setToast] = useState({
    open: false,
    message: "",
    severity: "success",
  });

  // Categories options
  const categories = [1, 2];

  useEffect(() => {
    const fetchEventData = async () => {
      try {
        const data = await fetchEventById(id);
        setFormData({
          ...data,
          startDate: new Date(data.startDate),
          endDate: new Date(data.endDate),
        });
      } catch (err) {
        setError(err.message || "Failed to load event");
      } finally {
        setLoading(false);
      }
    };

    fetchEventData();
  }, [id]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleDateChange = (name) => (newDate) => {
    setFormData((prev) => ({
      ...prev,
      [name]: newDate,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      setSubmitLoading(true);
      const updatedData = {
        ...formData,
        maxTicketCount: parseInt(formData.maxTicketCount, 10),
        category: parseInt(formData.category, 10),
      };

      const response = await updateEvent(id, updatedData);
      if (response === true) {
        setToast({
          open: true,
          message: "Event updated successfully!",
          severity: "success",
        });
        setTimeout(() => {
          navigate(`/eventview/${id}`);
        }, 1500);
      } else {
        setToast({
          open: true,
          message: `Failed to update event: ${response.statusText || 'Unknown error'}`,
          severity: "error",
        });
      }
    } catch (err) {
      setError(err.message || "Failed to update event");
      setToast({
        open: true,
        message: "Failed to update event",
        severity: "error",
      });
      console.error("Update error:", err);
    } finally {
      setSubmitLoading(false);
    }
  };

  const handleCloseToast = (event, reason) => {
    if (reason === "clickaway") {
      return;
    }
    setToast({ ...toast, open: false });
  };

  if (loading && !formData.name) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="100vh"
      >
        <CircularProgress sx={{ color: '#6a11cb' }} />
      </Box>
    );
  }

  if (error) {
    return (
      <Container maxWidth="md" sx={{ mt: 12, mb: 4 }}>
        <Alert severity="error">{error}</Alert>
      </Container>
    );
  }

  const categoryName = getCategoryName(formData.category);
  const imageUrl = getCategoryImage(formData.category);

  return (
    <>
      {/* Hero Section */}
      <Box 
        sx={{
          width: '100%',
          position: 'relative',
          height: { xs: '200px', md: '250px' },
          overflow: 'hidden',
          mt: 8,
        }}
      >
        <Box
          sx={{
            position: 'absolute',
            top: 0,
            left: 0,
            width: '100%',
            height: '100%',
            backgroundImage: `url(${imageUrl})`,
            backgroundSize: 'cover',
            backgroundPosition: 'center',
            '&::before': {
              content: '""',
              position: 'absolute',
              top: 0,
              left: 0,
              width: '100%',
              height: '100%',
              background: 'linear-gradient(to bottom, rgba(0,0,0,0.5) 0%, rgba(0,0,0,0.7) 100%)',
              zIndex: 1
            },
            '&::after': {
              content: '""',
              position: 'absolute',
              top: 0,
              left: 0,
              right: 0,
              bottom: 0,
              background: 'radial-gradient(circle at center, rgba(106,17,203,0.3) 0%, rgba(37,117,252,0) 70%)',
              zIndex: 1,
            }
          }}
        />
        <Container 
          maxWidth="lg"
          sx={{
            position: 'relative',
            zIndex: 2,
            height: '100%',
            display: 'flex',
            flexDirection: 'column',
            justifyContent: 'flex-end',
            pb: 4
          }}
        >
          <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
            <IconButton 
              component={Link}
              to={`/eventview/${id}`}
              sx={{ 
                color: 'white', 
                mr: 2,
                bgcolor: 'rgba(255,255,255,0.1)',
                '&:hover': {
                  bgcolor: 'rgba(255,255,255,0.2)'
                }
              }}
            >
              <ArrowBackIcon />
            </IconButton>
            <Chip 
              label={categoryName} 
              sx={{ 
                color: 'white',
                bgcolor: 'rgba(106,17,203,0.8)', 
                fontWeight: 600,
                px: 1
              }} 
            />
          </Box>

          <Typography 
            variant="h3" 
            component="h1"
            sx={{
              color: 'white',
              fontWeight: 700,
              textShadow: '0 2px 4px rgba(0,0,0,0.3)',
              fontSize: { xs: '1.8rem', sm: '2.2rem', md: '2.5rem' },
              lineHeight: 1.2,
              display: 'flex',
              alignItems: 'center',
              gap: 1.5
            }}
          >
            <EditIcon sx={{ fontSize: { xs: 24, md: 30 } }} /> Edit Event
          </Typography>
        </Container>
      </Box>

      <Container maxWidth="lg" sx={{ mt: { xs: -3, md: -5 }, mb: 8, position: 'relative', zIndex: 3 }}>
        <Card 
          sx={{
            borderRadius: 3,
            boxShadow: '0 10px 30px rgba(0,0,0,0.1)',
            overflow: 'hidden',
            p: 0
          }}
        >
          <CardContent sx={{ p: { xs: 3, md: 5 } }}>
            <form onSubmit={handleSubmit}>
              <Grid container spacing={4}>
                <Grid item xs={12}>
                  <Box sx={{ display: 'flex', alignItems: 'flex-start' }}>
                    <EventIcon sx={{ mt: 1.5, mr: 2, color: 'text.secondary' }} />
                    <TextField
                      fullWidth
                      label="Event Name"
                      name="name"
                      value={formData.name}
                      onChange={handleInputChange}
                      variant="standard"
                      required
                      sx={{
                        '& .MuiInputBase-input': {
                          fontSize: '1.5rem', 
                          fontWeight: 600,
                          lineHeight: 1.5
                        },
                      }}
                    />
                  </Box>
                </Grid>

                <Grid item xs={12}>
                  <Box sx={{ display: 'flex', alignItems: 'flex-start' }}>
                    <DescriptionIcon sx={{ mt: 1, mr: 2, color: 'text.secondary' }} />
                    <Paper 
                      elevation={0} 
                      sx={{ 
                        bgcolor: 'rgba(0,0,0,0.02)', 
                        p: 2, 
                        borderRadius: 2,
                        border: '1px solid rgba(0,0,0,0.06)',
                        width: '100%'
                      }}
                    >
                      <Typography 
                        variant="caption" 
                        component="label" 
                        sx={{ 
                          display: 'block', 
                          mb: 1, 
                          color: 'text.secondary',
                          fontSize: '0.85rem'
                        }}
                      >
                        Description
                      </Typography>
                      <TextField
                        fullWidth
                        name="description"
                        value={formData.description}
                        onChange={handleInputChange}
                        variant="standard"
                        multiline
                        rows={4}
                        required
                        placeholder="Write a description for your event..."
                        InputProps={{
                          disableUnderline: true,
                        }}
                        sx={{
                          '& .MuiInputBase-root': {
                            p: 0,
                          },
                          '& textarea': {
                            lineHeight: 1.6,
                          },
                        }}
                      />
                    </Paper>
                  </Box>
                </Grid>

                <Grid item xs={12}>
                  <Divider sx={{ my: 1 }} />
                </Grid>

                <Grid item xs={12} md={6}>
                  <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    <CategoryIcon sx={{ mr: 2, color: 'text.secondary' }} />
                    <FormControl fullWidth variant="outlined">
                      <InputLabel>Category</InputLabel>
                      <Select
                        name="category"
                        value={formData.category}
                        onChange={handleInputChange}
                        label="Category"
                        required
                        sx={{
                          '& .MuiOutlinedInput-notchedOutline': {
                            borderColor: 'rgba(106, 17, 203, 0.2)',
                          },
                          '&:hover .MuiOutlinedInput-notchedOutline': {
                            borderColor: 'rgba(106, 17, 203, 0.5)',
                          },
                          '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                            borderColor: '#6a11cb',
                          }
                        }}
                      >
                        {categories.map((category) => (
                          <MenuItem key={category} value={category}>
                            {getCategoryName(category)}
                          </MenuItem>
                        ))}
                      </Select>
                    </FormControl>
                  </Box>
                </Grid>

                <Grid item xs={12} md={6}>
                  <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    <TicketIcon sx={{ mr: 2, color: 'text.secondary' }} />
                    <TextField
                      fullWidth
                      label="Available Tickets"
                      name="maxTicketCount"
                      value={formData.maxTicketCount}
                      onChange={handleInputChange}
                      type="number"
                      variant="outlined"
                      required
                      InputProps={{
                        sx: { 
                          borderRadius: 2,
                        }
                      }}
                      sx={{
                        '& .MuiOutlinedInput-notchedOutline': {
                          borderColor: 'rgba(106, 17, 203, 0.2)',
                        },
                        '&:hover .MuiOutlinedInput-notchedOutline': {
                          borderColor: 'rgba(106, 17, 203, 0.5)',
                        },
                        '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                          borderColor: '#6a11cb',
                        }
                      }}
                    />
                  </Box>
                </Grid>

                <Grid item xs={12} md={6}>
                  <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    <CalendarIcon sx={{ mr: 2, color: 'text.secondary' }} />
                    <LocalizationProvider dateAdapter={AdapterDateFns}>
                      <DateTimePicker
                        label="Start Date"
                        value={formData.startDate}
                        onChange={handleDateChange("startDate")}
                        slotProps={{
                          textField: {
                            fullWidth: true,
                            required: true,
                            sx: {
                              '& .MuiOutlinedInput-notchedOutline': {
                                borderColor: 'rgba(106, 17, 203, 0.2)',
                                borderRadius: 2
                              },
                              '&:hover .MuiOutlinedInput-notchedOutline': {
                                borderColor: 'rgba(106, 17, 203, 0.5)',
                              },
                              '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                                borderColor: '#6a11cb',
                              }
                            }
                          },
                        }}
                      />
                    </LocalizationProvider>
                  </Box>
                </Grid>

                <Grid item xs={12} md={6}>
                  <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    <CalendarIcon sx={{ mr: 2, color: 'text.secondary' }} />
                    <LocalizationProvider dateAdapter={AdapterDateFns}>
                      <DateTimePicker
                        label="End Date"
                        value={formData.endDate}
                        onChange={handleDateChange("endDate")}
                        minDateTime={formData.startDate}
                        slotProps={{
                          textField: {
                            fullWidth: true,
                            required: true,
                            sx: {
                              '& .MuiOutlinedInput-notchedOutline': {
                                borderColor: 'rgba(106, 17, 203, 0.2)',
                                borderRadius: 2
                              },
                              '&:hover .MuiOutlinedInput-notchedOutline': {
                                borderColor: 'rgba(106, 17, 203, 0.5)',
                              },
                              '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                                borderColor: '#6a11cb',
                              }
                            }
                          },
                        }}
                      />
                    </LocalizationProvider>
                  </Box>
                </Grid>

                <Grid item xs={12}>
                  <Box
                    sx={{ 
                      borderTop: "1px solid", 
                      borderColor: "divider", 
                      pt: 4,
                      mt: 2,
                      display: 'flex',
                      justifyContent: 'space-between'
                    }}
                  >
                    <Button
                      variant="outlined"
                      color="inherit"
                      onClick={() => navigate(-1)}
                      startIcon={<ArrowBackIcon />}
                      sx={{ 
                        borderColor: 'rgba(0,0,0,0.23)',
                        color: 'text.secondary',
                        '&:hover': {
                          bgcolor: 'rgba(0,0,0,0.04)',
                          borderColor: 'text.primary'
                        },
                        px: 3
                      }}
                    >
                      Cancel
                    </Button>
                    <Button
                      type="submit"
                      variant="contained"
                      disabled={submitLoading}
                      startIcon={submitLoading ? <CircularProgress size={16} color="inherit" /> : <SaveIcon />}
                      sx={{ 
                        px: 4,
                        py: 1.2,
                        borderRadius: 2,
                        textTransform: 'none',
                        fontWeight: 600,
                        boxShadow: '0 3px 5px rgba(0,0,0,0.1)',
                        backgroundImage: 'linear-gradient(135deg, #6a11cb 0%, #2575fc 100%)',
                        '&:hover': {
                          boxShadow: '0 6px 10px rgba(0,0,0,0.15)',
                          transform: 'translateY(-1px)',
                        },
                        transition: 'all 0.2s ease'
                      }}
                    >
                      {submitLoading ? "Saving..." : "Save Changes"}
                    </Button>
                  </Box>
                </Grid>
              </Grid>
            </form>
          </CardContent>
        </Card>
      </Container>

      <Snackbar
        open={toast.open}
        autoHideDuration={3000}
        onClose={handleCloseToast}
        anchorOrigin={{ vertical: "top", horizontal: "center" }}
      >
        <MuiAlert
          onClose={handleCloseToast}
          severity={toast.severity}
          sx={{ width: "100%" }}
          elevation={6}
          variant="filled"
        >
          {toast.message}
        </MuiAlert>
      </Snackbar>
    </>
  );
}

export default EventEdit;
