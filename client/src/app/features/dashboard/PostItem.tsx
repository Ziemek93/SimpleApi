import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import React from 'react';

// type Props = {
//     post: PostListItem
// }
export default function PostItem(post: PostListItem) {

    const bull = (
        <Box
            component="span"
            sx={{ display: 'inline-block', mx: '2px', transform: 'scale(0.8)' }}
        >
            •
        </Box>
    );
    
    return (
        <Card sx={{ minWidth: 275 }}>
            <CardContent>
                <Typography variant="h2"  gutterBottom sx={{ color: 'text.secondary', fontSize: 14 }}>
                    {post.name}
                </Typography>
                <Typography>
                    Tags:
                </Typography>
                <Typography component="div" sx={{color: 'text.secondary', mb: 1}}>

                    {post.tags.map((tag, index) => (
                        <React.Fragment key={tag}>
                            {tag}
                            {index < post.tags.length - 1 && bull}
                        </React.Fragment>
                    ))}
                </Typography>
                <Typography>Category</Typography>
                <Typography variant="body2" sx={{color: 'text.secondary', mb: 1}}>
                    {post.category.name}
                </Typography>
            </CardContent>
            <CardActions>
                <Button size="small">Read more...</Button>
            </CardActions>
        </Card>
    );
}