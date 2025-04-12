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
    List as ListIcon,
    Home as HomeIcon,
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
        { title: 'Event List', path: '/eventlist', icon: <ListIcon /> },
    ];

    const isActive = (path) => {
        return location.pathname === path;
    };

    const handleDrawerToggle = () => {
        setDrawerOpen(!drawerOpen);
    };

    const drawer = (
        <Box onClick={handleDrawerToggle} sx={{ textAlign: 'center' }}>
            <Typography variant="h6" sx={{ my: 2 }}>
                EventPlus
            </Typography>
            <List>
                {navItems.map((item) => (
                    <ListItem key={item.title} disablePadding>
                        <ListItemButton
                            component={NavLink}
                            to={item.path}
                            sx={{
                                textAlign: 'left',
                                '&.active': {
                                    backgroundColor: theme.palette.primary.light,
                                },
                            }}
                            className={isActive(item.path) ? 'active' : ''}
                        >
                            <ListItemIcon sx={{ color: 'inherit' }}>
                                {item.icon}
                            </ListItemIcon>
                            <ListItemText primary={item.title} />
                        </ListItemButton>
                    </ListItem>
                ))}
            </List>
        </Box>
    );

    return (
        <AppBar position="fixed" color="default" elevation={0}
            style={{
                backgroundColor: 'rgba(255, 255, 255, 0.02)',
                WebkitBackdropFilter: 'blur(100px)',
                backdropFilter: 'blur(100px)',
            }}
        >
            <Container maxWidth="xl">
                <Toolbar disableGutters>
                    {isMobile ? (
                        <IconButton
                            edge="start"
                            color="inherit"
                            aria-label="open drawer"
                            onClick={handleDrawerToggle}
                            sx={{ mr: 2, display: { sm: 'none' } }}
                        >
                            <MenuIcon />
                        </IconButton>
                    ) : null}
                    <Typography
                        variant="h6"
                        component={NavLink}
                        to="/"
                        sx={{
                            mr: 2,
                            display: { xs: 'none', sm: 'block' },
                            fontFamily: 'monospace',
                            fontWeight: 700,
                            letterSpacing: '.3rem',
                            color: 'white',
                            textDecoration: 'none',
                        }}
                    >
                        EventPlus
                    </Typography>
                    <Box sx={{ flexGrow: 1, display: { xs: 'none', sm: 'flex' }, justifyContent: 'center' }}>
                        {!isMobile &&
                            navItems.map((item) => (
                                <Button
                                    key={item.title}
                                    component={NavLink}
                                    to={item.path}
                                    sx={{
                                        color: 'white',
                                        '&.active': {
                                            color: "magenta",
                                        },
                                    }}
                                    className={isActive(item.path) ? 'active' : ''}
                                >
                                    {item.title}
                                </Button>
                            ))}
                    </Box>
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                        <Button color="primary" sx={{ mr: 2, color: 'white', fontWeight: 700 }}>
                            Log In
                        </Button>
                        <Button color="primary" sx={{ mr: 2, color: 'white', fontWeight: 700 }}>
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
                    display: { xs: 'block', sm: 'none' },
                    '& .MuiDrawer-paper': { boxSizing: 'border-box', width: 240 },
                }}
            >
                {drawer}
            </Drawer>
        </AppBar>
    );
}

export default Navigation;