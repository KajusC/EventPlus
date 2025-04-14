import React from 'react';
import { NavLink, useLocation } from 'react-router-dom';
import {
    AppBar,
    Toolbar,
    Typography,
    Button,
    Container,
    Box,
    useMediaQuery,
    useTheme,
    IconButton,
    Drawer,
    List,
    ListItem,
    ListItemButton,
    ListItemIcon,
    ListItemText,
} from '@mui/material';
import {
    Menu as MenuIcon,
    Event as EventIcon,
    Add as AddIcon,
    Home as HomeIcon,
    ConfirmationNumber as TicketIcon
} from '@mui/icons-material';

function Navigation() {
    const location = useLocation();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('md'));
    const [drawerOpen, setDrawerOpen] = React.useState(false);

    const navItems = [
        { title: 'Home', path: '/', icon: <HomeIcon /> },
        { title: 'Events', path: '/events', icon: <EventIcon /> },
        { title: 'Add Event', path: '/eventinsert', icon: <AddIcon /> },
    ];

    const isActive = (path) => {
        return location.pathname === path;
    };

    const handleDrawerToggle = () => {
        setDrawerOpen(!drawerOpen);
    };

    const drawer = (
        <Box onClick={handleDrawerToggle} sx={{ textAlign: 'center', bgcolor: '#1a1a2e', color: 'white', height: '100%' }}>
            <Box sx={{ p: 2, borderBottom: '1px solid rgba(255,255,255,0.1)' }}>
                <Typography variant="h6" sx={{ my: 2, fontWeight: 700, letterSpacing: '1px' }}>
                    EventPlus
                </Typography>
            </Box>
            <List sx={{ pt: 2 }}>
                {navItems.map((item) => (
                    <ListItem key={item.title} disablePadding>
                        <ListItemButton
                            component={NavLink}
                            to={item.path}
                            sx={{
                                textAlign: 'left',
                                py: 1.5,
                                color: 'white',
                                '&.active': {
                                    background: 'linear-gradient(90deg, rgba(106,17,203,0.15) 0%, rgba(37,117,252,0.1) 100%)',
                                    borderLeft: '4px solid #6a11cb',
                                },
                                '&:hover': {
                                    background: 'rgba(255,255,255,0.05)',
                                }
                            }}
                            className={isActive(item.path) ? 'active' : ''}
                        >
                            <ListItemIcon sx={{ color: 'inherit', minWidth: '40px' }}>
                                {item.icon}
                            </ListItemIcon>
                            <ListItemText 
                                primary={item.title} 
                                primaryTypographyProps={{ 
                                    fontWeight: isActive(item.path) ? 600 : 400 
                                }}
                            />
                        </ListItemButton>
                    </ListItem>
                ))}
            </List>
        </Box>
    );

    return (
        <AppBar 
            position="fixed" 
            elevation={0}
            sx={{
                background: 'rgba(26, 26, 46, 0.8)',
                backdropFilter: 'blur(10px)',
                borderBottom: '1px solid rgba(255,255,255,0.1)'
            }}
        >
            <Container maxWidth="xl">
                <Toolbar disableGutters sx={{ height: '70px' }}>
                    {isMobile ? (
                        <IconButton
                            edge="start"
                            color="inherit"
                            aria-label="open drawer"
                            onClick={handleDrawerToggle}
                            sx={{ mr: 2 }}
                        >
                            <MenuIcon />
                        </IconButton>
                    ) : null}
                    
                    <Typography
                        variant="h6"
                        component={NavLink}
                        to="/"
                        sx={{
                            mr: 3,
                            display: 'flex',
                            alignItems: 'center',
                            fontFamily: 'Inter, system-ui, sans-serif',
                            fontWeight: 700,
                            fontSize: { xs: '1.1rem', md: '1.3rem' },
                            letterSpacing: '0.5px',
                            color: 'white',
                            textDecoration: 'none',
                            background: 'linear-gradient(45deg, #6a11cb 30%, #2575fc 90%)',
                            WebkitBackgroundClip: 'text',
                            WebkitTextFillColor: 'transparent',
                        }}
                    >
                        <EventIcon sx={{ mr: 1, fontSize: '28px' }} />
                        EventPlus
                    </Typography>
                    
                    <Box sx={{ flexGrow: 1, display: { xs: 'none', md: 'flex' }, justifyContent: 'center' }}>
                        {!isMobile &&
                            navItems.map((item) => (
                                <Button
                                    key={item.title}
                                    component={NavLink}
                                    to={item.path}
                                    startIcon={item.icon}
                                    sx={{
                                        mx: 1,
                                        py: 1,
                                        px: 2,
                                        color: 'white',
                                        fontWeight: 500,
                                        borderRadius: '8px',
                                        '&.active': {
                                            background: 'linear-gradient(90deg, rgba(106,17,203,0.1) 0%, rgba(37,117,252,0.1) 100%)',
                                            borderBottom: '2px solid #6a11cb',
                                        },
                                        '&:hover': {
                                            background: 'rgba(255,255,255,0.05)',
                                        }
                                    }}
                                    className={isActive(item.path) ? 'active' : ''}
                                >
                                    {item.title}
                                </Button>
                            ))}
                    </Box>
                    
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                        <Button 
                            variant="outlined"
                            sx={{ 
                                mr: 2, 
                                color: 'white', 
                                fontWeight: 600,
                                borderColor: 'rgba(255,255,255,0.3)',
                                '&:hover': {
                                    borderColor: 'white',
                                    background: 'rgba(255,255,255,0.05)'
                                }
                            }}
                        >
                            Log In
                        </Button>
                        <Button 
                            variant="contained"
                            sx={{ 
                                color: 'white', 
                                fontWeight: 600,
                                background: 'linear-gradient(45deg, #6a11cb 30%, #2575fc 90%)',
                                boxShadow: '0 3px 5px 2px rgba(106, 17, 203, .3)',
                                '&:hover': {
                                    boxShadow: '0 5px 8px 2px rgba(106, 17, 203, .35)',
                                }
                            }}
                        >
                            Sign Up
                        </Button>
                    </Box>
                </Toolbar>
            </Container>
            <Drawer
                variant="temporary"
                open={drawerOpen}
                onClose={handleDrawerToggle}
                ModalProps={{
                    keepMounted: true,
                }}
                sx={{
                    '& .MuiDrawer-paper': { 
                        width: 280,
                        boxShadow: '0 8px 10px -5px rgba(0,0,0,0.2),0 16px 24px 2px rgba(0,0,0,0.14),0 6px 30px 5px rgba(0,0,0,0.12)',
                    },
                }}
            >
                {drawer}
            </Drawer>
        </AppBar>
    );
}

export default Navigation;