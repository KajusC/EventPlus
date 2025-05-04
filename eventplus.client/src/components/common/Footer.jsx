import React from 'react';
import { Box, Container, Typography, Link, Grid, IconButton, Divider } from '@mui/material';
import { Facebook, Instagram, Twitter, YouTube } from '@mui/icons-material';

function Footer() {
    return (
        <Box
            component="footer"
            sx={{
                width: '100%',
                background: 'linear-gradient(to right, #1a1a2e, #16213e)',
                color: 'rgba(255, 255, 255, 0.8)',
                py: 6,
                mt: 'auto',
                borderTop: '1px solid rgba(255,255,255,0.1)',
            }}
        >
            <Container maxWidth="lg">
                <Grid container spacing={4}>
                    <Grid sx={{ width: { xs: '100%', md: '33.33%' } }}>
                        <Typography 
                            variant="h6" 
                            sx={{ 
                                mb: 2, 
                                fontWeight: 700,
                                fontSize: '1.5rem',
                                background: 'linear-gradient(45deg, #6a11cb 30%, #2575fc 90%)',
                                WebkitBackgroundClip: 'text',
                                WebkitTextFillColor: 'transparent',
                                display: 'inline-block'
                            }}
                        >
                            EventPlus
                        </Typography>
                        <Typography variant="body2" sx={{ mb: 2, opacity: 0.8, maxWidth: '300px' }}>
                            Your premier platform for discovering and booking the most exciting events. From music concerts to conferences, find what excites you.
                        </Typography>
                        <Box sx={{ mt: 3 }}>
                            <IconButton aria-label="Facebook" sx={{ mr: 1, color: '#4267B2' }}>
                                <Facebook />
                            </IconButton>
                            <IconButton aria-label="Twitter" sx={{ mr: 1, color: '#1DA1F2' }}>
                                <Twitter />
                            </IconButton>
                            <IconButton aria-label="Instagram" sx={{ mr: 1, color: '#C13584' }}>
                                <Instagram />
                            </IconButton>
                            <IconButton aria-label="YouTube" sx={{ color: '#FF0000' }}>
                                <YouTube />
                            </IconButton>
                        </Box>
                    </Grid>
                    
                    <Grid sx={{ width: { xs: '50%', md: '16.66%' } }}>
                        <Typography variant="h6" sx={{ mb: 2, fontWeight: 600, fontSize: '1rem' }}>
                            Company
                        </Typography>
                        <Link href="#" color="inherit" underline="hover" display="block" sx={{ mb: 1 }}>About Us</Link>
                        <Link href="#" color="inherit" underline="hover" display="block" sx={{ mb: 1 }}>Team</Link>
                        <Link href="#" color="inherit" underline="hover" display="block" sx={{ mb: 1 }}>Careers</Link>
                        <Link href="#" color="inherit" underline="hover" display="block" sx={{ mb: 1 }}>Contact</Link>
                    </Grid>
                    
                    <Grid sx={{ width: { xs: '50%', md: '16.66%' } }}>
                        <Typography variant="h6" sx={{ mb: 2, fontWeight: 600, fontSize: '1rem' }}>
                            Support
                        </Typography>
                        <Link href="#" color="inherit" underline="hover" display="block" sx={{ mb: 1 }}>Help Center</Link>
                        <Link href="#" color="inherit" underline="hover" display="block" sx={{ mb: 1 }}>Safety Center</Link>
                        <Link href="#" color="inherit" underline="hover" display="block" sx={{ mb: 1 }}>Community</Link>
                    </Grid>
                    
                    <Grid sx={{ width: { xs: '50%', md: '16.66%' } }}>
                        <Typography variant="h6" sx={{ mb: 2, fontWeight: 600, fontSize: '1rem' }}>
                            Legal
                        </Typography>
                        <Link href="#" color="inherit" underline="hover" display="block" sx={{ mb: 1 }}>Terms</Link>
                        <Link href="#" color="inherit" underline="hover" display="block" sx={{ mb: 1 }}>Privacy</Link>
                        <Link href="#" color="inherit" underline="hover" display="block" sx={{ mb: 1 }}>Cookies</Link>
                        <Link href="#" color="inherit" underline="hover" display="block" sx={{ mb: 1 }}>Licenses</Link>
                    </Grid>
                    
                    <Grid sx={{ width: { xs: '50%', md: '16.66%' } }}>
                        <Typography variant="h6" sx={{ mb: 2, fontWeight: 600, fontSize: '1rem' }}>
                            Resources
                        </Typography>
                        <Link href="#" color="inherit" underline="hover" display="block" sx={{ mb: 1 }}>Blog</Link>
                        <Link href="#" color="inherit" underline="hover" display="block" sx={{ mb: 1 }}>Newsletter</Link>
                        <Link href="#" color="inherit" underline="hover" display="block" sx={{ mb: 1 }}>Events</Link>
                        <Link href="#" color="inherit" underline="hover" display="block" sx={{ mb: 1 }}>FAQs</Link>
                    </Grid>
                </Grid>
                
                <Divider sx={{ my: 4, borderColor: 'rgba(255,255,255,0.1)' }} />
                
                <Box sx={{ display: 'flex', justifyContent: 'space-between', flexWrap: 'wrap', opacity: 0.7 }}>
                    <Typography variant="body2" sx={{ mb: { xs: 2, md: 0 } }}>
                        &copy; {new Date().getFullYear()} EventPlus. All rights reserved.
                    </Typography>
                    <Box>
                        <Link href="#" color="inherit" underline="hover" sx={{ mr: 3 }}>
                            Privacy Policy
                        </Link>
                        <Link href="#" color="inherit" underline="hover">
                            Terms of Service
                        </Link>
                    </Box>
                </Box>
            </Container>
        </Box>
    );
}

export default Footer;