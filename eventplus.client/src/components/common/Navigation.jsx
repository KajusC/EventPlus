import React from 'react';
import { NavLink, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
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
    Avatar,
    Menu,
    MenuItem,
} from '@mui/material';
import {
    Menu as MenuIcon,
    Event as EventIcon,
    Add as AddIcon,
    Home as HomeIcon,
    Person as PersonIcon,
    Logout as LogoutIcon,
    Login as LoginIcon,
    PersonAdd as PersonAddIcon,
    Dashboard as DashboardIcon,
    EditCalendar as EditCalendarIcon
} from '@mui/icons-material';

function Navigation() {
    const location = useLocation();
    const navigate = useNavigate();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('md'));
    const [drawerOpen, setDrawerOpen] = React.useState(false);
    const [anchorEl, setAnchorEl] = React.useState(null);
    const { currentUser, logout, isAuthenticated, isAdmin, isOrganizer } = useAuth();

    const handleOpenMenu = (event) => {
        setAnchorEl(event.currentTarget);
    };

    const handleCloseMenu = () => {
        setAnchorEl(null);
    };

    const handleLogout = () => {
        logout();
        handleCloseMenu();
        navigate('/');
    };

    const handleProfileClick = () => {
        handleCloseMenu();
        navigate('/profile');
    };

    const navItems = [
        { title: 'Home', path: '/', icon: <HomeIcon />, public: true },
        { title: 'Events', path: '/events', icon: <EventIcon />, public: true },
        { title: 'Create Event', path: '/eventinsert', icon: <AddIcon />, public: false, adminOnly: false, organizerOnly: true },
        { title: 'Manage Events', path: '/myevents', icon: <EditCalendarIcon />, public: false, adminOnly: false, organizerOnly: true },
    ];

    if (isAdmin()) {
        navItems.push({ 
            title: 'Admin Dashboard', 
            path: '/admin', 
            icon: <DashboardIcon />, 
            public: false, 
            adminOnly: true 
        });
    }

    const filteredNavItems = navItems.filter(item => {
        if (item.public) return true;
        if (!isAuthenticated()) return false;
        if (item.adminOnly && !isAdmin()) return false;
        if (item.organizerOnly && !isOrganizer() && !isAdmin()) return false;
        return true;
    });

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
                {filteredNavItems.map((item) => (
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
                {!isAuthenticated() && (
                    <>
                        <ListItem disablePadding>
                            <ListItemButton
                                component={NavLink}
                                to="/login"
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
                                className={isActive('/login') ? 'active' : ''}
                            >
                                <ListItemIcon sx={{ color: 'inherit', minWidth: '40px' }}>
                                    <LoginIcon />
                                </ListItemIcon>
                                <ListItemText primary="Log In" />
                            </ListItemButton>
                        </ListItem>
                        <ListItem disablePadding>
                            <ListItemButton
                                component={NavLink}
                                to="/register"
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
                                className={isActive('/register') ? 'active' : ''}
                            >
                                <ListItemIcon sx={{ color: 'inherit', minWidth: '40px' }}>
                                    <PersonAddIcon />
                                </ListItemIcon>
                                <ListItemText primary="Sign Up" />
                            </ListItemButton>
                        </ListItem>
                    </>
                )}
                {isAuthenticated() && (
                    <>
                        <ListItem disablePadding>
                            <ListItemButton
                                component={NavLink}
                                to="/profile"
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
                                className={isActive('/profile') ? 'active' : ''}
                            >
                                <ListItemIcon sx={{ color: 'inherit', minWidth: '40px' }}>
                                    <PersonIcon />
                                </ListItemIcon>
                                <ListItemText primary="Profile" />
                            </ListItemButton>
                        </ListItem>
                        <ListItem disablePadding>
                            <ListItemButton
                                onClick={handleLogout}
                                sx={{
                                    textAlign: 'left',
                                    py: 1.5,
                                    color: 'white',
                                    '&:hover': {
                                        background: 'rgba(255,255,255,0.05)',
                                    }
                                }}
                            >
                                <ListItemIcon sx={{ color: 'inherit', minWidth: '40px' }}>
                                    <LogoutIcon />
                                </ListItemIcon>
                                <ListItemText primary="Logout" />
                            </ListItemButton>
                        </ListItem>
                    </>
                )}
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
                            filteredNavItems.map((item) => (
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
                        {!isAuthenticated() ? (
                            <>
                                <Button 
                                    variant="outlined"
                                    component={NavLink}
                                    to="/login"
                                    startIcon={<LoginIcon />}
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
                                    component={NavLink}
                                    to="/register"
                                    startIcon={<PersonAddIcon />}
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
                            </>
                        ) : (
                            <>
                                <IconButton
                                    onClick={handleOpenMenu}
                                    sx={{ 
                                        p: 0,
                                        border: '2px solid rgba(255,255,255,0.2)',
                                        '&:hover': {
                                            border: '2px solid rgba(255,255,255,0.3)',
                                        }
                                    }}
                                >
                                    <Avatar sx={{ bgcolor: 'primary.main' }}>
                                        {currentUser.name ? currentUser.name.charAt(0).toUpperCase() : 'U'}
                                    </Avatar>
                                </IconButton>
                                <Menu
                                    id="menu-appbar"
                                    anchorEl={anchorEl}
                                    anchorOrigin={{
                                        vertical: 'bottom',
                                        horizontal: 'right',
                                    }}
                                    keepMounted
                                    transformOrigin={{
                                        vertical: 'top',
                                        horizontal: 'right',
                                    }}
                                    open={Boolean(anchorEl)}
                                    onClose={handleCloseMenu}
                                    sx={{
                                        mt: 1,
                                        '& .MuiPaper-root': {
                                            backgroundColor: '#1a1a2e',
                                            color: 'white',
                                            borderRadius: '8px',
                                            boxShadow: '0 8px 16px rgba(0,0,0,0.2)',
                                        }
                                    }}
                                >
                                    <Box sx={{ p: 2, textAlign: 'center', borderBottom: '1px solid rgba(255,255,255,0.1)' }}>
                                        <Typography variant="subtitle1" sx={{ fontWeight: 600 }}>
                                            {currentUser.name} {currentUser.surname}
                                        </Typography>
                                        <Typography variant="body2" sx={{ color: 'rgba(255,255,255,0.6)' }}>
                                            {currentUser.role}
                                        </Typography>
                                    </Box>
                                    <MenuItem onClick={handleProfileClick} sx={{ py: 1.5 }}>
                                        <ListItemIcon sx={{ color: 'white' }}>
                                            <PersonIcon fontSize="small" />
                                        </ListItemIcon>
                                        <Typography>Profile</Typography>
                                    </MenuItem>
                                    <MenuItem onClick={handleLogout} sx={{ py: 1.5 }}>
                                        <ListItemIcon sx={{ color: 'white' }}>
                                            <LogoutIcon fontSize="small" />
                                        </ListItemIcon>
                                        <Typography>Logout</Typography>
                                    </MenuItem>
                                </Menu>
                            </>
                        )}
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