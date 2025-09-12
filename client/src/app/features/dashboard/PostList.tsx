import {usePosts} from "../../../lib/hooks/usePosts.ts";
import {Grid} from "@mui/material";
import PostItem from "./PostItem.tsx";
import Typography from "@mui/material/Typography";


export default function PostList(){
    const { posts, isPendingPosts } = usePosts();
    
    if (!posts || isPendingPosts) return <Typography>Loading...</Typography>

    return (
        <Grid container spacing={2} sx={{ justifyContent: 'center', maxWidth: '75%', display: 'flex', flexDirection: 'row', flexWrap: 'wrap', gap: 2 }}>
            {posts.map(post => (
                <Grid key={post.id} size={{xs:12, sm:6, md:6}}>
                        <PostItem {...post} />
                </Grid>
            ))}
        </Grid>
    )
}